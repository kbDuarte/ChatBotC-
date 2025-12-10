using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chatIA
{
    public partial class Form1 : Form
    {
        private const string ModelName = "gemini-2.0-flash";

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            lblErro.Text = "";
            txtSaida.Text = "";


            string apiKey = "AIzaSyAwZJOotZ_QuCtmetgQQ11_g0RaZoeB0FY";

            if (string.IsNullOrWhiteSpace(txtEntrada.Text))
            {
                lblErro.Text = "Digite uma mensagem na entrada.";
                return;
            }

            //historico papai
            txtHistorico.AppendText("Você: " + txtEntrada.Text + Environment.NewLine);


            //chamando o gemini aq
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url =
                        $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:generateContent?key={apiKey}";

                    var body = new
                    {
                        contents = new[]
                        {
                            new {
                                parts = new[] { new { text = txtEntrada.Text } },
                                role = "user"
                            }
                        }
                    };

                    //serializando a resposta 
                    string jsonBody = JsonSerializer.Serialize(body);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseText = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        lblErro.Text = "Erro da IA: " + responseText;
                        return;
                    }

                    // exibindo a resposta dps de extraida e capturada
                    string textoGerado = ExtrairTexto(responseText);

                    txtSaida.Text = textoGerado;

                    // historico da ia
                    txtHistorico.AppendText("IA: " + textoGerado + Environment.NewLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                lblErro.Text = "Erro: " + ex.Message;
            }

            txtEntrada.Text = "";
        }

        
        //extraindo o json da resposta
        private string ExtrairTexto(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);

                var candidates = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text");

                return candidates.GetString();
            }
            catch
            {
                return "Erro ao interpretar a resposta da IA.";
            }
        }

        private void txtSaida_TextChanged(object sender, EventArgs e)
        {
            txtSaida.ReadOnly = true;
        }
    }
}
