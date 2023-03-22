Shader "FTK/ClipableHologram"
{
    Properties
    {
        [HDR]
        _Color("Color", Color) = (0, 0.5, 1, 1)
        _Tint("Tint", Range(0, 1)) = 0.5
        [Space]
        _WireThickness("Wire Thickness", Float) = 0.0005
        _VertexThickness("Vertex Thickness", Float) = 4
        _WireSmoothing("Wire Smoothing", Float) = 0.002
        [HDR]
        [Space]
        _EdgeColor("Edge Color", Color) = (1, 0.5, 0, 1)
        _EdgeWidth("Edge Width", Float) = 0.08
        _EdgeSmoothing("Edge Smoothing", Float) = 0.0005
        [Space]
        [MaterialToggle]
        _ClippingEnabled("Clipping Enabled", Int) = 1
        _ClipPlane("Clipping Plane", Vector) = (0, 1, 0, -0.16)
        [Space]
        _NearFadeBegin("Near Fade Begin", Float) = 0.4
        _NearFadeEnd("Near Fade End", Float) = 0.2
        _AlphaMultiplier("Alpha Multiplier", Range(0, 1)) = 1
        [Space]
        [MaterialToggle]
        _UniformScaling("Assume Uniform Scaling", Int) = 1
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend OneMinusDstColor One //soft add
            //Blend One OneMinusSrcAlpha //premul tranparency
            ZWrite Off
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma geometry geom
                #pragma fragment frag

                #include "UnityCG.cginc"

                float4 _ClipPlane;
                float4 _Color;
                float4 _EdgeColor;
                float _WireThickness;
                float _Tint;
                float _WireSmoothing;
                float _EdgeWidth;
                float _NearFadeBegin;
                float _NearFadeEnd;
                float _VertexThickness;
                float _EdgeSmoothing;
                float _AlphaMultiplier;
                bool _ClippingEnabled;
                bool _UniformScaling;
                const float epsilon = 1e-75;

                struct appdata
                {
                    float4 objPos : POSITION;
                    float4 objNormal : NORMAL;
                    float4 objTangent : TANGENT;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2g
                {
                    float4 worldPos : POSITION1;
                    float4 clipPos : SV_POSITION;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                struct g2f
                {
                    float4 worldPos : COLOR;
                    float4 clipPos : SV_POSITION;
                    float3 normal : NORMAL;
                    float3 tangent : TANGENT;
                    float4 wireRef : TEXCOORD;
                    float wireWeight : DEPTH;
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
                    o.tangent = normalize(_UniformScaling ? UnityObjectToWorldDir(v.objTangent) : UnityObjectToWorldNormal(v.objTangent));
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
                    //Front Side of Tri
                    for (int i = 0; i < 3; i++)
                    {
                        UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], o[i]);
                        o[i].clipPos = input[i].clipPos;
                        o[i].worldPos = input[i].worldPos;
                        o[i].normal = input[i].normal;
                        o[i].tangent = input[i].tangent;
                        o[i].wireWeight = 1;
                    }
                    o[0].wireRef = float4(distsToSides[0], 0, 0, 0);
                    o[1].wireRef = float4(0, distsToSides[1], 0, 0);
                    o[2].wireRef = float4(0, 0, distsToSides[2], 0);
                    triStream.Append(o[0]);
                    triStream.Append(o[1]);
                    triStream.Append(o[2]);
                    triStream.RestartStrip();
                    //Back Side of Tri
                    for (int j = 3; j < 6; j++)
                    {
                        UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], o[j]);
                        o[j].clipPos = input[j - 3].clipPos;
                        o[j].worldPos = input[j - 3].worldPos;
                        o[j].normal = -input[j - 3].normal;
                        o[j].tangent = -input[j - 3].tangent;
                        o[j].wireWeight = _VertexThickness;
                    }
                    o[3].wireRef = float4(0, sideLengths[2], sideLengths[1], 0);
                    o[4].wireRef = float4(sideLengths[2], 0, sideLengths[0], 0);
                    o[5].wireRef = float4(sideLengths[1], sideLengths[0], 0, 0);
                    triStream.Append(o[5]);
                    triStream.Append(o[4]);
                    triStream.Append(o[3]);
                    triStream.RestartStrip();
                }

                float4 frag(g2f i) : SV_Target
                { UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                    float3 viewRay = i.worldPos - _WorldSpaceCameraPos;
                    float3 viewDir = normalize(viewRay);
                    float distToCam = length(viewRay);

                    _WireThickness = max(_WireThickness * i.wireWeight * 0.0005, epsilon);
                    _EdgeWidth = max(_EdgeWidth * 0.001, epsilon);
                    _WireSmoothing = max(_WireSmoothing * distToCam, epsilon);
                    _EdgeSmoothing = max(_EdgeSmoothing * distToCam * 0.001, epsilon);

                    float wireAlpha = min(min(i.wireRef[0], i.wireRef[1]), i.wireRef[2]) / _WireThickness - 1;
                    wireAlpha = saturate(1 - wireAlpha / _WireSmoothing);

                    float fresnelAlpha = 1 + dot(viewDir, i.normal);
                    fresnelAlpha = saturate(1 - fresnelAlpha * fresnelAlpha);

                    float alpha = wireAlpha * fresnelAlpha + (1 - wireAlpha) * (1 - fresnelAlpha);

                    float4 geomCol = float4(abs(i.normal + i.tangent) * 0.5, _Color.w);
                    float4 col = (_Tint * _Color + (1 - _Tint) * geomCol);
                    col.w *= alpha;

                    float edge = (dot(i.worldPos, normalize(_ClipPlane.xyz)) - _ClipPlane.w) * _ClippingEnabled;

                    float edgeColWeight = saturate(1 - edge / _EdgeWidth) * _ClippingEnabled;
                    edgeColWeight *= edgeColWeight * _EdgeColor.w;

                    col += _EdgeColor * edgeColWeight;
                    float fade = saturate((distToCam - _NearFadeEnd) / max(_NearFadeBegin - _NearFadeEnd, epsilon));
                    col *= fade * fade;
                    col *= saturate(edge / _EdgeSmoothing) + 1 - _ClippingEnabled;
                    col.xyz *= col.w * _AlphaMultiplier;

                    clip(col);
                    return saturate(col);
                }
                ENDCG
            }
        }
}
