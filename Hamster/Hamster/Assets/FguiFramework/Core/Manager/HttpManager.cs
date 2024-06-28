using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;

public class HttpManager : Singleton<HttpManager>
{
    private static HttpClientHandler handler;
    private CookieContainer cookieContainer;
    private HttpClient client;

    public delegate void HttpCallBack(string res);

    public override void Init()
    {
        handler = new HttpClientHandler() { UseCookies = true };
        cookieContainer = new CookieContainer(); 
        handler.CookieContainer = cookieContainer;
        handler.UseCookies = true;
        client = new HttpClient(handler);
    }

    public async Task GetAsync(string url, HttpCallBack callback = null)
    {
        var response = await client.GetStringAsync(url);
        callback?.Invoke(response);
    }

    public async Task PostAsync(string url, string str, HttpCallBack callback = null)
    { 
        HttpContent content = new StringContent(str);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();
        callback?.Invoke(responseString);
    }
}
