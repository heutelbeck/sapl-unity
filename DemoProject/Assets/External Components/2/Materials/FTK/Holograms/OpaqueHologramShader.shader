Shader "FTK/OpaqueHologramShader"
{
    Properties
    {
        [HDR]
        _PrimaryColor("Primary Color", Color) = (0.5, 0.5, 0.5, 1)
        [HDR]
        _SecondaryColor("Secondary Color", Color) = (0, 0.5, 1, 1)
        _Tint("Tint", Range(0, 1)) = 0.5
        [Space]
        _WireThickness("Wire Thickness", Float) = 0.25
        _VertexThickness("Vertex Thickness", Float) = 0.5
        _Smoothing("Mesh Smoothing", Float) = 15
        [Space]
        [MaterialToggle]
        _ClippingEnabled("Clipping Enabled", Int) = 1
        [HDR]
        _EdgeColor("Edge Color", Color) = (1, 0.5, 0, 1)
        _EdgeThickness("Edge Thickness", Float) = 0.5
        _ClipPlane("Clipping Plane", Vector) = (0, 1, 0, -0.125)
        [Space]
        _NearFadeBegin("Near Fade Begin", Float) = 0.2
        _NearFadeEnd("Near Fade End", Float) = 0.1
        _LightnessMultiplier("Lightness Multiplier", Range(0, 1)) = 1
        [Space]
        [MaterialToggle]
        _UniformScaling("Assume Uniform Scaling", Int) = 1
    }
    SubShader
    {
        Tags {"RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ClipPlane;
            float4 _PrimaryColor;
            float4 _SecondaryColor;
            float4 _EdgeColor;
            float _WireThickness;
            float _Tint;
            float _Smoothing;
            float _NearFadeBegin;
            float _NearFadeEnd;
            float _VertexThickness;
            float _LightnessMultiplier;
            float _EdgeThickness;
            bool _ClippingEnabled;
            bool _UniformScaling;

            struct appdata
            {
                float4 objPos : POSITION;
                float4 objNormal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2g
            {
                float4 worldPos : POSITION1;
                float4 clipPos : SV_POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct g2f
            {
                float4 worldPos : COLOR;
                float4 clipPos : SV_POSITION;
                float3 normal : NORMAL;
                float4 wireRef : TEXCOORD0;
                float4 vertexRef : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float distToLine(float3 p, float3 a, float3 b) {
                float3 lin = b - a;
                float3 dir = normalize(lin);
                float3 hypo = p - a;
                float3 proj = a + dot(hypo, dir) * dir;
                return length(proj - p);
            }

            v2g vert(appdata v)
            {
                v2g o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.clipPos = UnityObjectToClipPos(v.objPos);
                o.worldPos = mul(unity_ObjectToWorld, v.objPos);
                o.normal = normalize(_UniformScaling ? UnityObjectToWorldDir(v.objNormal) : UnityObjectToWorldNormal(v.objNormal));
                return o;
            }

            [maxvertexcount(6)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);
                g2f o[6];
                float3 sideLengths = float3(length(input[1].worldPos - input[2].worldPos),
                    length(input[0].worldPos - input[2].worldPos),
                    length(input[0].worldPos - input[1].worldPos));
                float3 distsToSides = float3(
                    distToLine(input[0].worldPos, input[1].worldPos, input[2].worldPos),
                    distToLine(input[1].worldPos, input[2].worldPos, input[0].worldPos),
                    distToLine(input[2].worldPos, input[0].worldPos, input[1].worldPos)
                    );
                for (int i = 0; i < 3; i++)
                {
                    UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], o[i]);
                    o[i].clipPos = input[i].clipPos;
                    o[i].worldPos = input[i].worldPos;
                    o[i].normal = input[i].normal;
                }
                for (int j = 3; j < 6; j++)
                {
                    UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], o[j]);
                    o[j].clipPos = input[j - 3].clipPos;
                    o[j].worldPos = input[j - 3].worldPos;
                    o[j].normal = -input[j - 3].normal;
                }
                o[0].wireRef = o[3].wireRef = float4(distsToSides[0], 0, 0, 0);
                o[1].wireRef = o[4].wireRef = float4(0, distsToSides[1], 0, 0);
                o[2].wireRef = o[5].wireRef = float4(0, 0, distsToSides[2], 0);
                o[0].vertexRef = o[3].vertexRef = float4(0, sideLengths[2], sideLengths[1], 0);
                o[1].vertexRef = o[4].vertexRef = float4(sideLengths[2], 0, sideLengths[0], 0);
                o[2].vertexRef = o[5].vertexRef = float4(sideLengths[1], sideLengths[0], 0, 0);
                triStream.Append(o[0]);
                triStream.Append(o[1]);
                triStream.Append(o[2]);
                triStream.RestartStrip();
                triStream.Append(o[5]);
                triStream.Append(o[4]);
                triStream.Append(o[3]);
                triStream.RestartStrip();
            }

            float4 frag(g2f i) : SV_Target
            {
                float epsilon = 1e-19f;
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float3 viewRay = i.worldPos - _WorldSpaceCameraPos;
                float3 viewDir = normalize(viewRay);
                float distToCam = length(viewRay);

                float fresnelValue = 1 + dot(viewDir, i.normal);
                fresnelValue = saturate(1 - fresnelValue * fresnelValue);

                _WireThickness = max(_WireThickness * 0.0005f, epsilon);
                _VertexThickness = max(_VertexThickness * 0.001f, epsilon);
                _EdgeThickness = max(_EdgeThickness * 0.001f, epsilon);
                _Smoothing = max(_Smoothing * distToCam * (1 - 0.5f * fresnelValue), epsilon);

                float wireValue = min(min(i.wireRef[0], i.wireRef[1]), i.wireRef[2]) / _WireThickness - 1;
                float vertexValue = min(min(i.vertexRef[0], i.vertexRef[1]), i.vertexRef[2]) / _VertexThickness - 1;

                float meshValue = saturate(1 - min(wireValue, vertexValue) / _Smoothing);
                float geometryValue = meshValue * fresnelValue + (1 - meshValue) * (1 - fresnelValue);
                geometryValue *= geometryValue;

                float4 geomCol = float4(abs(i.normal * 0.5f + 0.5f), 1) * length(_SecondaryColor.xyz);
                float4 col = (_Tint * _SecondaryColor + (1 - _Tint) * geomCol);
                col = (1 - geometryValue) * _PrimaryColor + geometryValue * col;

                float edge = (dot(i.worldPos, normalize(_ClipPlane.xyz)) - _ClipPlane.w) * _ClippingEnabled;
                float edgeValue = saturate(1 - (edge / _EdgeThickness - 1) / _Smoothing) * _ClippingEnabled;
                col = (1 - edgeValue) * col + edgeValue * _EdgeColor;

                float fade = saturate((distToCam - _NearFadeEnd) / max(_NearFadeBegin - _NearFadeEnd, epsilon));
                col *= fade * fade;
                col *= _LightnessMultiplier;

                col = saturate(col);
                clip(saturate(edge * fade) + 1 - _ClippingEnabled - epsilon);
                return col;
            }
            ENDCG
        }
    }
}
