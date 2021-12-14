using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

class Program
{
    public struct InfoRede
    {
        public string ip;
        public string nome;
        public string descricao;
    }

    public static InfoRede ObterRedePadrao()
    {
        InfoRede retorno = new InfoRede() { ip = string.Empty, nome = string.Empty, descricao = string.Empty };

        // obtem uma matriz com todas as interfaces de rede (geralmente uma por placa de rede, dialup, e conexão VPN)
        NetworkInterface[] interfacesRede = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface rede in interfacesRede)
        {
            // lê a configuração de IP para cada rede
            IPInterfaceProperties properties = rede.GetIPProperties();

            if (rede.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                   rede.OperationalStatus == OperationalStatus.Up &&
                   !rede.Description.ToLower().Contains("virtual") &&
                   !rede.Description.ToLower().Contains("pseudo"))
            {
                // cada rede pode ter multiplos IP
                foreach (IPAddressInformation endereco in properties.UnicastAddresses)
                {
                    // nós queremos apenas o tipo IPv4 e vamos ignorar loopback (127.0.0.1)
                    if (endereco.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(endereco.Address))
                    {
                        // mais detalhes
                        retorno.ip = endereco.Address.ToString();
                        retorno.nome = rede.Name;
                        retorno.descricao = rede.Description;
                        break;
                    }
                }
            }
        }

        return retorno;
    }

    static void Main(string[] args)
    {
        InfoRede rede = ObterRedePadrao();
        Console.WriteLine($"IP       : {rede.ip}\nNome     : {rede.nome}\nDescrição: {rede.descricao}");
        Console.ReadLine();

        /* a saída será algo como:
           IP       : 192.168.60.152
           Nome     : Ethernet
           Descrição: Realtek PCIe GbE Family Controller        
        */
    }
}