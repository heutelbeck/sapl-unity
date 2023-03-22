using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using FaPra.ScriptableObjects;
using EvtSource;
using Sandbox.Damian.FTK.Data;
using Sandbox.Damian.FTK.Data.Entities;
using Sandbox.Damian.FTK.Data.Serialization;
using Sandbox.Damian.FTK.Network.Clients;
using System;
using System.Collections.Concurrent;
using static EvtSource.EventSourceReader;
using FTK.Network;
using Unity.VisualScripting;

public class NewRESTClient 
{
    private static string exampleJSONBU = @"[
                                        {'id':'BuildUnit2','type':'buildUnit','name':'Build Unit 2'},
                                        {'id':'BuildUnit3','type':'buildUnit','name':'Build Unit 3'}
                                        ]";

    static HttpMethod httpMethod;
    static Uri uri;
    static HttpRequestMessage response;

    static string link;
    public static List<Device> getProcessingStations() {
        HttpClient client = new HttpClient();
        BackendSettings settings = MasterSettingsProvider.Instance.SelectedSetting;
        link = settings.Protocol + "://" +
                        settings.Host + ":" +
                        settings.Port + 
                        settings.ApiProcessingStations;
        Debug.Log("using new rest");
        
        httpMethod= HttpMethod.Post;
        uri= new Uri(link);

        //var sendRequest = Task.Run(() => NetworkUtils.SendRequestAsync(httpMethod, uri));
        //Debug.Log (sendRequest.Result);

        // Backend
        //var task = Task.Run(() => client.GetAsync("http://localhost:8080/api/processingstations"));
        var task = Task.Run(() => client.GetAsync(link));
        task.Wait();
        var result = task.Result;

        var reader = Task.Run(() => result.Content.ReadAsStringAsync());
        reader.Wait();
        var json = reader.Result;

        

        List<Device> devices2 = JsonConvert.DeserializeObject<List<Device>>(json);


        // Test
        //List <Device> devices2 = JsonConvert.DeserializeObject<List<Device>>(exampleJSON2);

        return devices2;
    }

    public static List<Device> getBuildUnits() {

        List<Device> devices2 = JsonConvert.DeserializeObject<List<Device>>(exampleJSONBU);

        return devices2;
    }


    
}
