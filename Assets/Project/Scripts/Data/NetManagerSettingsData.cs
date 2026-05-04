using UnityEngine;

namespace TRPG.Runtime
{
    [CreateAssetMenu(fileName = "SO_NetManagerSettings", menuName = "Scriptable Objects/Settings/Net Manager")]
    public class NetManagerSettingsData : ScriptableObject
    {
        [Header("TCP")]
        [SerializeField] private string tcpHost = "192.168.0.3";
        [SerializeField] private int tcpPort = 60000;


        public string TcpHost => tcpHost;
        public int TcpPort => tcpPort;


        [Header("HTTP")]
        [SerializeField] private string httpHost = "192.168.0.3";
        [SerializeField] private int httpPort = 60000;


        public string HttpHost => httpHost;
        public int httphost => httpPort;
    }
}
