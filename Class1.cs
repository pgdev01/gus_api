using System.Runtime.InteropServices;
using gus_api.API;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace gus_api
{
    public class Class1
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string WypiszPodmioty(string klucz, string nip)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            EndpointAddress address = new EndpointAddress("https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");

            // Stwórz klienta i zaloguj się
            UslugaBIRzewnPublClient klient = new UslugaBIRzewnPublClient(binding, address);
            string sid = klient.Zaloguj(klucz);

            // Dodaj wymagany nagłówek HTTP
            OperationContextScope scope = new OperationContextScope(klient.InnerChannel);
            HttpRequestMessageProperty reqProps = new HttpRequestMessageProperty();
            reqProps.Headers.Add("sid", sid);
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = reqProps;

            // Odbierz dane na podstawie NIP
            ParametryWyszukiwania parametryGr1 = new ParametryWyszukiwania();
            parametryGr1.Nip = nip;

            string response = klient.DaneSzukajPodmioty(parametryGr1);

            klient.Wyloguj(sid);

            return response;
        }
    }
}