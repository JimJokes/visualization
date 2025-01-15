using UnityEngine;
using MikeSchweitzer.WebSocket;
using Baracuda.Monitoring;
using System.Collections.Generic;
using DG.Tweening;

public class BackendSocket : MonitoredBehaviour
{
    #region Websocket
    public WebSocketConnection connection;
    public string url = "ws://localhost:37522"; // ETS2LA + 2 ports
    public float connection_retry_time = 5.0f; // seconds
    private float last_connection_retry = 0;
    #endregion

    #region Profiling
    public float profiling_time = 1.0f;
    [Monitor]
    private string connection_status = "Disconnected";
    [Monitor]
    private int packets_per_second = 0;
    private float last_profile = 0;
    private List<string> messages = new List<string>();
    #endregion

    #region Backend Setup
    public int[] subscribed_channels = new int[] { 1, 2, 3 };

    class SubscribeChannel {
        public int channel;
        public string method;
    }
    
    [System.Serializable]
    public class BaseResult {
        public string type;
        public object data;
    }
    [System.Serializable]
    public class BaseResponse {
        public int channel;
        public BaseResult result;
    }
    #endregion

    # region Channel 1
    [System.Serializable]
    public class Transform {
        public float x;
        public float y;
        public float z;
        public float rx;
        public float ry;
        public float rz;
    }
    [System.Serializable]
    public class TransformResult : BaseResult { public new Transform data; }
    [System.Serializable]
    public class TransformResponse : BaseResponse { public new TransformResult result; }
    #endregion

    # region Channel 2
    [System.Serializable]
    public class SteeringData
    {
        public Point[] points;
    }
    [System.Serializable]
    public class SteeringResult : BaseResult { public new SteeringData data; }
    [System.Serializable]
    public class SteeringResponse : BaseResponse { public new SteeringResult result; }
    #endregion

    # region Channel 3
    [System.Serializable]
    public class TruckState
    {
        public float speed;
        public float speed_limit;
        public float cruise_control;
        public float target_speed;
        public float throttle;
        public float brake;
    }

    [System.Serializable]
    public class StateResult : BaseResult { public new TruckState data; }
    [System.Serializable]
    public class StateResponse : BaseResponse { public new StateResult result; }
    #endregion

    # region Truck
    public class Truck{
        public Transform transform;
        public Vector3[] steering;
        public TruckState state;
    }
    public Truck truck = new Truck();
    #endregion 

    public void Connect()
    {
        last_connection_retry = Time.time;
        Debug.LogWarning("Connecting to " + url);
        connection.Connect(url);
    }

    public void Disconnect()
    {
        connection.Disconnect();
    }

    private new void Awake()
    {
        DOTween.SetTweensCapacity(2000, 100);
        connection.StateChanged += OnStateChanged;
        connection.ErrorMessageReceived += OnErrorMessageReceived;
        Connect();
        this.StartMonitoring();
    }

    private new void OnDestroy()
    {
        connection.StateChanged -= OnStateChanged;
        connection.ErrorMessageReceived -= OnErrorMessageReceived;
        this.StopMonitoring();
    }

    private void OnConnect(WebSocketConnection connection){
        Debug.Log("Successfully connected to " + url);
        Debug.Log("Connecting to " + subscribed_channels.Length + " channels...");
        for (int i = 0; i < subscribed_channels.Length; i++)
        {
            SubscribeChannel subscribe = new SubscribeChannel();
            subscribe.channel = subscribed_channels[i];
            subscribe.method = "subscribe";
            
            connection.AddOutgoingMessage(JsonUtility.ToJson(subscribe));
            Debug.Log("Subscribed to channel " + subscribed_channels[i]);
        }
    }

    private void OnStateChanged(WebSocketConnection connection, WebSocketState oldState, WebSocketState newState)
    {
        switch (newState)
        {
            case WebSocketState.Connected:
                connection_status = "Connected";
                OnConnect(connection);
                break;
            case WebSocketState.Disconnected:
                connection_status = "Disconnected";
                Connect();
                break;
            case WebSocketState.Invalid:
                connection_status = "Invalid";
                break;
        }
    }

    private void OnErrorMessageReceived(WebSocketConnection connection, string errorMessage)
    {
        Debug.LogError("Socket failed to connect. Error: " + errorMessage);
    }

    void Update()
    {
        if (connection.State == WebSocketState.Disconnected && Time.time - last_connection_retry > connection_retry_time)
        {
            Debug.LogError("Connection lost, retrying...");
            Connect();
        }

        while (connection.TryRemoveIncomingMessage(out string message)){
            BaseResponse response = JsonUtility.FromJson<BaseResponse>(message);
            switch (response.channel)
            {
                case 1:
                    TransformResponse truck_response = JsonUtility.FromJson<TransformResponse>(message);
                    TransformResult truck_result = truck_response.result;
                    truck.transform = truck_result.data;
                    break;

                case 2:
                    SteeringResponse steering_response = JsonUtility.FromJson<SteeringResponse>(message);
                    SteeringResult steering_result = steering_response.result;
                    try
                    {
                        Vector3[] points = new Vector3[steering_result.data.points.Length];
                        for (int i = 0; i < steering_result.data.points.Length; i++)
                        {
                            points[i] = steering_result.data.points[i].ToVector3() + Vector3.up * 0.1f;
                        }
                        truck.steering = points;
                    } catch {}

                    break;

                case 3:
                    StateResponse state_response = JsonUtility.FromJson<StateResponse>(message);
                    StateResult state_result = state_response.result;
                    truck.state = state_result.data;
                    break;
                    
            }

            messages.Add(message);
        }

        if (Time.time - last_profile > profiling_time)
        {
            packets_per_second = messages.Count;
            messages.Clear();
            last_profile = Time.time;
        }
    }

}
