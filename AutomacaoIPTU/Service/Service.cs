using AutomacaoIPTU.Repository;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;
using AutomacaoIPTU.Models;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Interactions;
using System.Net;
using TwoCaptcha.Captcha;
using OpenQA.Selenium.Support.UI;
using CheckData;
using OpenQA.Selenium.Support.Extensions;
using static AutomacaoIPTU.Models.Municipio;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Configuration;
using System.Xml.Linq;
using Keys = OpenQA.Selenium.Keys;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Microsoft.VisualBasic.ApplicationServices;
using BitMiracle.Docotic.Pdf;
using System.Reflection.PortableExecutable;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using PdfDocument = BitMiracle.Docotic.Pdf.PdfDocument;
using UglyToad.PdfPig.Graphics;
using System.Runtime.InteropServices;

namespace AutomacaoIPTU.Service
{
    internal class Service
    {
        readonly ChromeOptions options = new();
        readonly Proxy proxy = new();
        ChromeDriver? driver;
        List<Iptu> _solicitacao = new();
        Repository.Repository _repository = new Repository.Repository();
        private TwoCaptcha.TwoCaptcha? _solver;
        public int id = 0;



        public async Task<Captchas> SolveCaptcha(string imagePath)
        {
            // Create a Normal captcha instance and set options
            Captchas ReturnCaptcha = new Captchas();
            _solver = new TwoCaptcha.TwoCaptcha("8308b24eb8b7ba7aed62703c06ae249d");
            //_solver = new TwoCaptcha.TwoCaptcha("2406528a22450e6b335f91832195abd0");
            var captcha = new Normal();
            captcha.SetFile(imagePath);
            captcha.SetMinLen(4);
            captcha.SetMaxLen(20);
            captcha.SetCaseSensitive(true);
            captcha.SetLang("en");
            double balance = await _solver.Balance();
            if (balance <= 5)
            {
                bool Enviado = _repository.ConsultarAvisoSaldo();
                if (Enviado)
                    _repository.EnviarAvisoSaldo();
            }
            try
            {
                // Solve the captcha
                await _solver.Solve(captcha);

                // Print the solved captcha code
                ReturnCaptcha.Code = captcha.Code;
                ReturnCaptcha.Id = captcha.Id;

                //File.Delete(imagePath);

                // Return the solved captcha code
                return ReturnCaptcha;
            }
            catch (Exception e)
            {
                // Handle errors
                Console.WriteLine("Error occurred: " + e.Message);
                // You might want to log or handle the error differently
                throw;
            }
        }

        public async Task<string> ContestCaptcha(string? id)
        {
            // Create a Normal captcha instance and set options
            Captchas ReturnCaptcha = new Captchas();
            _solver = new TwoCaptcha.TwoCaptcha("2406528a22450e6b335f91832195abd0");
            try
            {
                await _solver.Report(id, false);
                return "Sucesso";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public Task<string> SolveImgCaptcha(string imagePath)
        {
            // Create a Normal captcha instance and set options
            _solver = new TwoCaptcha.TwoCaptcha("2406528a22450e6b335f91832195abd0");
            Coordinates captcha = new Coordinates();
            captcha.SetFile(imagePath);
            captcha.SetLang("en");
            try
            {
                _solver.Solve(captcha).Wait();
                Console.WriteLine("Captcha solved: " + captcha.Code);
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Error occurred: " + e.InnerExceptions.First().Message);
            }
            return Task.FromResult(captcha.Code);
        }



        public async Task<string> SolveImagemCaptcha()
        {
            // Create a Normal captcha instance and set options
            _solver = new TwoCaptcha.TwoCaptcha("8308b24eb8b7ba7aed62703c06ae249d");
            //var apiKey = "2406528a22450e6b335f91832195abd0";
            var apiKey = "8308b24eb8b7ba7aed62703c06ae249d";
            var siteKey = "6Ld5csgUAAAAALeZPIZo1Pz3K_nTqI8P0ZHLK4kx";
            var captcha = new Normal();
            captcha.SetMinLen(4);
            captcha.SetMaxLen(20);
            captcha.SetCaseSensitive(true);
            captcha.SetLang("en");
            double balance = await _solver.Balance();
            if (balance <= 5)
            {
                bool Enviado = _repository.ConsultarAvisoSaldo();
                if (Enviado)
                    _repository.EnviarAvisoSaldo();
            }

            try
            {

                using (HttpClient client = new())
                {
                    // Construa a URL da API 2Captcha
                    string apiUrl = $"http://2captcha.com/in.php?key={apiKey}&method=userrecaptcha&googlekey={siteKey}&pageurl=http://example.com";

                    // Envie a solicitação para obter o ID do captcha
                    string response = await client.GetStringAsync(apiUrl);

                    // Extraia o ID do captcha da resposta
                    string captchaId = response.Split('|')[1];


                    // Construa a URL da API 2Captcha para obter a resposta do captcha
                    string getResultUrl = $"http://2captcha.com/res.php?key={apiKey}&action=get&id={captchaId}";


                    // Envie a solicitação para obter a resposta do captcha
                    bool pronto = true;
                    while (pronto)
                    {
                        Thread.Sleep(3000);
                        response = await client.GetStringAsync(getResultUrl);
                        if (!response.Contains("CAPCHA_NOT_READY"))
                            pronto = false;
                    }
                    // response = client.DownloadString(getResultUrl);

                    // Extraia a resposta do captcha da resposta
                    string captchaResponse = response.Split('|')[1];

                    return captchaResponse;
                }
            }
            catch (Exception e)
            {
                // Handle errors
                Console.WriteLine("Error occurred: " + e.Message);
                // You might want to log or handle the error differently
                throw;
            }
        }


        // Método auxiliar para obter o atributo personalizado do enum
        static MunicipioInfoAttribute GetMunicipioInfoAttribute(Municipio municipio)
        {
            var field = municipio.GetType().GetField(municipio.ToString());
            var attribute = (MunicipioInfoAttribute)Attribute.GetCustomAttribute(field, typeof(MunicipioInfoAttribute));
            return attribute;
        }

        public Dictionary<string, Func<List<Iptu>, object, Task>> RetornoLista(List<Iptu> Solicitacao)
        {
            var servicosPorMunicipio = new Dictionary<string, Func<List<Iptu>, object, Task>>
            {
                        { "GUA" , async(s, d) => await NavegacaoGuarulhos(Solicitacao, driver) },
                        { "ITA" , async(s, d) => await NavegacaoItaqua(Solicitacao, driver) },
                        { "SUZ" , async(s, d) => await NavegacaoSuzano(Solicitacao,driver) },
            };

            return servicosPorMunicipio;
        }

        public void MatarProcessos()
        {
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            Process[] chrome = Process.GetProcessesByName("chrome");
            foreach (var chromeProcess in chrome)
            {
                chromeProcess.Kill();
            }
        }
        static int Modulo97(string numero)
        {
            const int modulo = 97;
            int tamanho = numero.Length;
            const int sliceLength = 7; // Tamanho do bloco a ser processado

            long resto = 0;
            for (int i = 0; i < tamanho; i += sliceLength)
            {
                string bloco = i + sliceLength <= tamanho ? numero.Substring(i, sliceLength) : numero.Substring(i);
                resto = long.Parse(resto.ToString() + bloco) % modulo;
            }

            return (int)resto;
        }

        public string CalcularDV(string numeroProcesso)
        {
            // Concatenação do número do processo com "00" para representar o DV desconhecido
            string numeroParaCalculo = numeroProcesso + "00";
            long numeroParaVerificacao = long.Parse(numeroParaCalculo);

            // Cálculo do DV
            int resto = Modulo97(numeroParaVerificacao.ToString());
            int dv = 98 - resto;

            string dvFormatado = dv.ToString("00"); // Formata o dígito verificador com dois dígitos


            // Formatação do DV para ter sempre dois dígitos
            return dv.ToString("00");
        }
        public void ArrumarProcessos()
        {
            _solicitacao = Repository.Repository.RetornaSolicitacoesAll();
            foreach (var Solicitacao in _solicitacao)
            {
                string New;
                string Old;
                if (Solicitacao.Processo.Trim().Length == 19)
                {
                    Old = Solicitacao.Processo.Trim();
                    var DV = Solicitacao.Processo.Substring(17, 2);
                    Solicitacao.Processo = Solicitacao.Processo.Substring(0, Solicitacao.Processo.Trim().Length - 2);
                    New = "0" + Solicitacao.Processo.Substring(0, 6) + DV + Solicitacao.Processo.Substring(6, 11);
                    Repository.Repository.CorrigirProcesso(New, Old);

                }

            }

        }

        public void GerarCodigoMunicipios()
        {
            _solicitacao = Repository.Repository.RetornaSolicitacoesOrg();
            if (_solicitacao.Count > 0)
            {
                Dictionary<string, (string Nome, int Codigo)> municipioDictionary = new();
                foreach (Municipio municipio in Enum.GetValues(typeof(Municipio)))
                {
                    var municipioInfoAttribute = GetMunicipioInfoAttribute(municipio);
                    if (municipioInfoAttribute != null)
                    {
                        var chave = municipioInfoAttribute.Nome.ToUpper();
                        var valor = (municipioInfoAttribute.Nome, municipioInfoAttribute.Codigo);

                        municipioDictionary[chave] = valor;
                    }
                }


                foreach (var Solicitacao in _solicitacao)
                {
                    var chaveBusca = Solicitacao.Cidade.Replace(" ", "").ToUpper();
                    if (municipioDictionary.TryGetValue(chaveBusca, out var valorEncontrado))
                        _repository.InserirOrgTabela(valorEncontrado.Codigo.ToString(), Solicitacao.NumeroInscricao);

                }
            }
        }
        public Task NavegacaoGuarulhos(List<Iptu> Solicitacao, IWebDriver? driver)
        {
            string? url = null;
            bool pular = true;
            try
            {
                foreach (var solicitacao in Solicitacao)
                {
                    //var RetirarFila = _repository.RetornaExcluido(solicitacao.CodigoLote.Trim());
                    //if (RetirarFila)
                    //{
                    //    _repository.RetirarFilaSemCPF(solicitacao.NumeroInscricao);
                    //    continue;
                    //}

                    if (pular)
                    {
                        MatarProcessos();
                        driver = IniciarChrome();
                    }
                    pular = true;
                    Actions actions = new Actions(driver);
                    driver.Url = "https://fazenda.guarulhos.sp.gov.br/ords/guarulho/f?p=628:LOGIN::::::";
                    IWebElement tipoinscricao = null;
                    for (int i = 0; i < 20; i++)
                    {
                        Thread.Sleep(600);
                        tipoinscricao = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[1]/td[2]/select");
                        if (tipoinscricao != null) { break; }
                    }

                    var selectElement = new SelectElement(tipoinscricao);
                    selectElement.SelectByValue("I");
                    var inscricao = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[2]/td[2]/input"));
                    if (inscricao != null)
                        inscricao.Click();

                    if (solicitacao.NumeroInscricao.Trim().Contains("__"))
                    {
                        // solicitacao.NumeroInscricao = solicitacao.NumeroInscricao.Substring(0, 14) + "00000";
                        pular = false;
                        _repository.RetirarFilaInscricaoErrada(solicitacao.NumeroInscricao);
                        continue;
                    }

                    driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[2]/td[2]/input")).SendKeys(solicitacao.NumeroInscricao.Replace(" ", ""));
                    var BtnLogin = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[2]/tbody/tr/td[3]/input"));
                    if (BtnLogin != null)
                        BtnLogin.Click();

                    var inexistente = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/div/span[2]/div");
                    if (inexistente != null)
                    {
                        //processo inexistente

                        _repository.RetirarFilaNaoEncontrado(solicitacao.NumeroInscricao.Trim());
                        pular = false;
                        continue;
                    }
                    Thread.Sleep(500);
                    actions.SendKeys(Keys.End).Build().Perform();
                    for (int i = 0; i < 5; i++)
                    {
                        actions.SendKeys(Keys.Up).Build().Perform();
                    }
                    IWebElement? IconSecund = null;
                    IWebElement? clickLink = null;
                    IWebElement? Selectano = null;
                    for (int i = 0; i < 40; i++)
                    {
                        Thread.Sleep(300);
                        IconSecund = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table[3]/tbody/tr/td/div/table[1]/tbody/tr[4]/td[4]/span/div/div[1]/a/img");
                        if (IconSecund != null) { break; }
                    }
                    if (IconSecund != null)
                        IconSecund.Click();
                    for (int i = 0; i < 40; i++)
                    {
                        Thread.Sleep(300);
                        clickLink = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table[1]/tbody/tr[2]/td/table/tbody/tr[1]/td/span[1]");
                        if (clickLink != null) { break; }
                    }
                    if (clickLink != null)
                        clickLink.Click();

                    for (int i = 0; i < 40; i++)
                    {
                        Thread.Sleep(300);
                        Selectano = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[2]/tbody/tr/td[2]/select");
                        if (clickLink != null) { break; }
                    }
                    var selectElemente = new SelectElement(Selectano);
                    string? UltimoAno = null;
                    var tableAno = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[2]");
                    IList<IWebElement> tableRowsAno = tableAno.FindElements(By.TagName("tr"));
                    IList<IWebElement> rowTDsAno;
                    foreach (IWebElement row in tableRowsAno)
                    {
                        rowTDsAno = row.FindElements(By.TagName("td"));
                        foreach (var title in rowTDsAno)
                        {
                            var Verifica = title.Text.Split("R$");
                            if (!Verifica.Contains("Exercício"))
                                Parallel.ForEach(Verifica, teste =>
                                {
                                    var valor = teste.Split("\r\n");
                                    UltimoAno = valor.Last();
                                });

                        }
                    }
                    solicitacao.Titular = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/div/div[3]/table[2]/tbody/tr[1]/td[2]/span")).Text;
                    if (!string.IsNullOrEmpty(UltimoAno))
                    {
                        selectElemente.SelectByValue(UltimoAno);
                        var BtnEmit = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[3]/tbody/tr/td[2]/input"));
                        if (BtnEmit != null)
                        {
                            var ttt = BtnEmit.Text;
                            BtnEmit.Click();
                            Thread.Sleep(2000);
                            try
                            {
                                IList<string> tabs = new List<string>(driver.WindowHandles);
                                for (int i = 0; i < 4; i++)
                                {
                                    driver.SwitchTo().Window(tabs[i]);
                                    url = driver.Url;
                                    int ntirar = driver.Url.Count();
                                    if (ntirar > 19)
                                        if (url.Substring(ntirar - 18, 18).Contains("P10_TIP_DICA:R"))
                                            driver.Close();
                                }
                                IList<string> tabss = new List<string>(driver.WindowHandles);
                                for (int i = 0; i < 4; i++)
                                {
                                    driver.SwitchTo().Window(tabss[i]);
                                    url = driver.Url;
                                    int ntirar = driver.Url.Count();
                                    if (ntirar > 35)
                                        if (url.Substring(ntirar - 35, 35).Contains("P4_TIPO_CERTIDAO"))
                                            break;
                                    Thread.Sleep(400);
                                }
                                Thread.Sleep(2000);

                                driver.Manage().Window.Maximize();
                                Thread.Sleep(600);
                                driver.ExecuteJavaScript("window.focus();");
                                Thread.Sleep(600);
                                IJavaScriptExecutor jss = (IJavaScriptExecutor)driver;
                                jss.ExecuteScript("window.print = function(){ return false;};");


                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("setTimeout(function() { window.print(); }, 0);");
                                driver.SwitchTo().Window(driver.WindowHandles.Last());
                                Thread.Sleep(1000);
                                string JSPath = "document.querySelector('body>print-preview-app').shadowRoot.querySelector('#sidebar').shadowRoot.querySelector('print-preview-button-strip').shadowRoot.querySelector('cr-button.cancel-button')";
                                IWebElement cancelBtn = (IWebElement)js.ExecuteScript($"return {JSPath}");
                                cancelBtn.Click();


                                Thread.Sleep(600);

                            }
                            catch { }

                            string valor = null;
                            driver.SwitchTo().Window(driver.WindowHandles.Last());
                            driver.Manage().Window.Maximize();
                            Thread.Sleep(1000);
                            var ttable = EsperarElemento(driver, "XPath", "/html/body/form/table[2]/tbody/tr/td[1]/div/table[3]");
                            if (ttable != null)
                            {
                                IList<IWebElement> tableRows = ttable.FindElements(By.TagName("tr"));
                                IList<IWebElement> rowTDs;
                                foreach (IWebElement row in tableRows)
                                {
                                    rowTDs = row.FindElements(By.TagName("td"));
                                    foreach (var title in rowTDs)
                                    {
                                        var ddd = title.Text.Split("R$");
                                        if (ddd[0].Length > 11)
                                            foreach (var teste in ddd)
                                            {
                                                valor = teste.Substring(0, 11);
                                                valor = valor.Replace(" ", "");
                                                bool apenasNumeros = Regex.IsMatch(valor, @"^[\d.,]+$");
                                                if (apenasNumeros)
                                                {

                                                    solicitacao.VlrVenal = valor;
                                                }
                                            }
                                    }
                                }
                            }
                            driver.Close();
                        }

                    }
                    Thread.Sleep(1000);
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    driver.Url = "https://fazenda.guarulhos.sp.gov.br/ords/guarulho/f?p=628:LOGIN::::::";
                    for (int i = 0; i < 20; i++)
                    {
                        Thread.Sleep(600);
                        tipoinscricao = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[1]/td[2]/select");
                        if (tipoinscricao != null) { break; }
                    }
                    selectElement = new SelectElement(tipoinscricao);
                    selectElement.SelectByValue("I");
                    inscricao = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[2]/td[2]/input"));
                    if (inscricao != null)
                    {
                        inscricao.Click();
                    }
                    //driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[2]/td[2]/input")).SendKeys("082.24.74.0323.00.000");
                    driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[1]/tbody/tr[2]/td[2]/input")).SendKeys(solicitacao.NumeroInscricao.Replace(" ", ""));
                    BtnLogin = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table/tbody/tr[2]/td/table[2]/tbody/tr/td[3]/input"));
                    if (BtnLogin != null)
                    {
                        BtnLogin.Click();
                    }
                    Thread.Sleep(1000);

                    actions.SendKeys(Keys.End).Build().Perform();
                    for (int i = 0; i < 3; i++)
                    {
                        actions.SendKeys(Keys.Up).Build().Perform();
                    }
                    Thread.Sleep(1000);
                    var Icon2 = driver.FindElement(By.XPath("/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/table[3]/tbody/tr/td/div/table[1]/tbody/tr[3]/td[6]/span/div/div[1]/a/img"));
                    if (Icon2 != null)
                    {
                        Icon2.Click();
                    }
                    IWebElement table = null;
                    for (int i = 0; i < 20; i++)
                    {
                        Thread.Sleep(5000);
                        table = EsperarElemento(driver, "XPath", "/html/body/form/div/div[2]/table/tbody/tr/td[1]/div/div/table/tbody/tr[5]/td/table");
                        if (table != null) { break; }
                    }
                    IList<IWebElement> tableRow = table.FindElements(By.TagName("tr"));
                    IList<IWebElement> rowTD;
                    IMain check = new Main();
                    string? Compar = null;
                    List<Iptu> ObjList = new List<Iptu>();
                    foreach (IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        foreach (var title in rowTD)
                        {
                            Compar = title.Text;
                            bool valid = check.IsNumber(Compar);
                            if (valid)
                            {
                                //castin do objeto
                                Iptu Obj = new Iptu();
                                {
                                    Obj.Ano = rowTD[0].Text;
                                    Obj.Tipo = rowTD[1].Text;
                                    Obj.Recibo = rowTD[2].Text;
                                    Obj.VlPrincipal = rowTD[3].Text;
                                    Obj.Correcao = rowTD[4].Text;
                                    Obj.Multa = rowTD[5].Text;
                                    Obj.Juros = rowTD[6].Text;
                                    Obj.Honorarios = rowTD[7].Text;
                                    Obj.Custas = rowTD[8].Text;
                                    Obj.Total = rowTD[9].Text;
                                    Obj.Processo = rowTD[10].Text;
                                    Obj.Status = rowTD[11].Text;
                                }
                                Obj.Titular = solicitacao.Titular;
                                Obj.NumeroInscricao = solicitacao.NumeroInscricao.Trim();
                                Obj.VlrVenal = solicitacao.VlrVenal;

                                if (!string.IsNullOrEmpty(Obj.Processo))
                                {
                                    Obj.Processo = Obj.Processo.Replace("/", "") + "8260224";
                                    if (Obj.Processo.Length < 19)
                                    {
                                        var DV = CalcularDV(Obj.Processo);
                                        Obj.Processo = "0" + Obj.Processo;
                                        var FaltForOnze = Obj.Processo.Length - 11;
                                        Obj.Processo = Obj.Processo.Substring(0, 7) + DV + Obj.Processo.Substring(FaltForOnze, 11);
                                    }
                                    Obj.Processo = Obj.Processo.Substring(0, 7) + "-" + Obj.Processo.Substring(7, 2) + "." + Obj.Processo.Substring(9, 4) + "." + Obj.Processo.Substring(13, 1) + "." + Obj.Processo.Substring(14, 2) + "." + Obj.Processo.Substring(16, 4);
                                }
                                ObjList.Add(Obj);
                                break;
                            }
                        }

                    }
                    //chamada sql para registro no banco
                    if (ObjList.Count > 0)
                        _repository.InserirTextoTabelaFila(ObjList);
                    else
                    {
                        if (solicitacao.VlrVenal != null)
                            _repository.InserirValorVenal(solicitacao);
                    }
                    _repository.Processado(solicitacao.NumeroInscricao.Trim());
                }
                MatarProcessos();
            }

            catch (Exception ex)
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoIPTU\ScreenShot\{ticks}.jpg");
                //screenshot.SaveAsFile(@$"C:\Users\FlávioSilvaVanquishC\Downloads\{ticks}.jpg");
                Solicitacao.First().Log_Erro = ex.Message;
                driver.Quit();
                throw;
            }

            return Task.CompletedTask;
        }

        public IWebDriver? IniciarChrome()
        {
            ChromeOptions options = new();
            Proxy proxy = new();

            try
            {
                MatarProcessos();
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-default-apps");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-site-isolation-trials");
                options.AddArgument("--disable-logging");
                options.AddArgument("--log-level=3");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-bundled-ppapi-flash");
                options.AddArgument("--disable-gpu-compositing");
                options.AddArgument("--disable-gpu-shader-disk-cache");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--window-size=600,600");
                options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
                //config responsavel por setar uma porta, sem essa config é possivel abrir varias instancias da automação 
                //options.AddArgument("--remote-debugging-port=9223");
                //options.DebuggerAddress = "127.0.0.1:5555";
                options.PageLoadStrategy = PageLoadStrategy.None;
                options.AddAdditionalChromeOption("useAutomationExtension", false);
                options.AddUserProfilePreference("disable-popup-blocking", true);
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("no-sandbox");
                options.AddArgument("test-type");
                //options.AddUserProfilePreference("download.default_directory", @"C:\Users\FlávioSilvaVanquishC\Desktop"); // Pasta onde os downloads serão salvos
                options.AddExcludedArguments(new List<string>() { "enable-automation" });
                proxy.IsAutoDetect = false;
                options.Proxy = proxy;
                var driverService = ChromeDriverService.CreateDefaultService(@"F:\AutomacaoAndamento\chromeDrive");
                //var driverService = ChromeDriverService.CreateDefaultService(@"C:\Users\FlávioSilvaVanquishC\Downloads");
                driver = new ChromeDriver(driverService, options);
                driver.Manage().Window.Maximize();
                id = driverService.ProcessId;
                return driver;
            }
            catch (Exception ex)
            {
                driver.Quit();
                throw;

            }
        }

        public async Task NavegacaoSuzano(List<Iptu> Solicitacao, IWebDriver driver)
        {
            bool pular = false;
            Actions actions = new Actions(driver);
            try
            {
                foreach (var solicitacao in Solicitacao)
                {
                    //var RetirarFila = _repository.RetornaExcluido(solicitacao.CodigoLote.Trim());
                    //if (RetirarFila)
                    //{
                    //    _repository.RetirarFilaSemCPF(solicitacao.NumeroInscricao);
                    //    continue;
                    //}
                    MatarProcessos();

                    driver = IniciarChrome();
                    Thread.Sleep(3000);
                    driver.SwitchTo().Window(driver.WindowHandles.LastOrDefault());
                    //driver.Navigate().Refresh();
                    driver.Url = "https://grp.suzano.sp.gov.br/portalcidadao/#78c3e513dd43cb27d8a3e2f376196ffc656d7ea577b2c6fb1650baea0%C4%B0370d00458b634d8e5a638d0ba41d3d7cc274988f21bc625395a3aeb2818b8b198662bf4efc9a81f0f975cbae9b9fbd2d0b4843db91c1103a1c219a970653df178064c0403d5d5b6fafeb8bf28a790ff42edf7bc55f9305e3c5e950d4c43ee3a3ba970b2b78f5e2806ce9ff306951856d7469390002e3d38905a578acdf45c78abbd603d963056b72bc27d68452480c9785f11c54c8d248c338ceedc983a7f686ef366f0b8419248827547789eb189bc7e0aa04f0c43c3565d067055fc65a6370012c78";

                    for (int i = 0; i < 400; i++)
                    {
                        Thread.Sleep(300);
                        var chech = EsperarElemento(driver, "XPath", "/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[2]/input");
                        if (chech != null) { break; }
                    }

                    driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[2]/input")).SendKeys(solicitacao.NumeroInscricao.Trim());
                    var solvedCaptchaCode = await SolveImagemCaptcha();
                    driver.ExecuteJavaScript($"document.getElementById('g-recaptcha-response').innerHTML='{solvedCaptchaCode}';");
                    var BtnBusc = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[5]/button"));
                    if (BtnBusc != null)
                        BtnBusc.Click();

                    var VerificaHumano = driver.FindElements(By.XPath("/html/body/div[2]/div[3]/div[1]/div/div/span/div[1]")).FirstOrDefault();
                    if (VerificaHumano != null)
                    {
                        VerificaHumano.Click();
                        Thread.Sleep(3000);
                    }


                    Thread.Sleep(25000);
                    try
                    {
                        driver.SwitchTo().Alert().Accept();
                    }
                    catch { }
                    driver.SwitchTo().Window(driver.WindowHandles.First());
                    IList<string> tabs = new List<string>(driver.WindowHandles);
                    if (tabs.Count == 1)
                    {
                        Thread.Sleep(90000);
                        tabs = new List<string>(driver.WindowHandles);
                        if (tabs.Count <= 1)
                        {
                            try
                            {
                                driver.SwitchTo().Alert().Accept();
                                pular = true;

                            }
                            catch { }
                        }
                    }
                    if (!pular)
                    {
                        driver.SwitchTo().Window(tabs[1]);
                        using (var client = new WebClient())
                        {
                            byte[] data = client.DownloadData(driver.Url);

                            using (var pdf = new BitMiracle.Docotic.Pdf.PdfDocument(data))
                            {
                                string text = pdf.GetText();
                                var textoTrat = text.Split("\r\n");
                                string valorvem;
                                foreach (var trat in textoTrat)
                                {
                                    if (trat.Contains("Proprietário:"))
                                    {
                                        int sub = trat.Length - 13;
                                        var Proprietario = trat.Substring(13, sub);
                                        solicitacao.Titular = Proprietario;

                                    }
                                    if (trat.Contains("Valor  Venal do Imóvel:"))
                                    {
                                        valorvem = trat.Substring(24, 9);
                                        _repository.InserirValorVenalProp(solicitacao.NumeroInscricao, solicitacao.Titular, valorvem);
                                        _repository.Processado(solicitacao.NumeroInscricao.Trim());
                                        solicitacao.VlrVenal = valorvem;
                                    }

                                }

                            }
                        }
                        driver.SwitchTo().Window(tabs[1]);
                        driver.Close();
                        driver.SwitchTo().Window(tabs[0]);
                        Thread.Sleep(7000);
                    }
                    pular = false;
                    driver.Url = "https://grp.suzano.sp.gov.br/portalcidadao/#78c3e513dd43cb27d8a3e2f376196ffc656d7ea577b2c6fbe5d%C4%B1282f5ab8f8cddc4a9fa15e6ef00c78b32e7180120906495294caff7f0dfec48badc153094b3c43d87555431253c625186c9b60aeef43820c9af98db531424c6476aaf0d110e6c6827a74da2556db6b1ee0f68b9057614c24260da6fd0c700a8c6635e0f9d9505300eab3ae01b3533c3c4432689c170bd691105e313756cf85b16c09e268411c6b36f8af6bfaf6fb25abadafea894e036c11dc65ac9467f27700217f09b097c7c25917bd9d67b63a99db18dbb63b93a0bdb13214459cd905426225a0d4cd776a01194aca660e2e54";
                    Thread.Sleep(7000);
                    var BtnImovel = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[1]/span[2]/input"));
                    if (BtnImovel.Displayed)
                        BtnImovel.Click();
                    //driver.Navigate().Refresh();
                    //string CodLoteamento = null;
                    //List<string> ListCpfVinculado = new List<string>();
                    //List<string> ListCpfVinculadoRetirar = new List<string>();
                    ////Logica de Busca Por cpfs
                    //var LisCPF = _repository.RetornaCPF(solicitacao.CodigoLoteamentoLoteador.Trim());
                    //ListCpfVinculado.AddRange(_repository.RetornaCPFVinculado(solicitacao.CodigoLote.Trim()));
                    //if (LisCPF.Count > 0)
                    //{
                    //    foreach (var Tentativas in LisCPF)
                    //    {
                    //        var ret = _repository.RetornaCPFPorLoteador(Tentativas);
                    //        if (ret.Trim() != null)
                    //            ListCpfVinculado.Add(ret);
                    //    }
                    //}
                    //foreach (var VerficaNull in ListCpfVinculado)
                    //{
                    //    if (string.IsNullOrEmpty(VerficaNull))
                    //        ListCpfVinculadoRetirar.Add(VerficaNull);
                    //}
                    //foreach (var VerficaNull in ListCpfVinculadoRetirar)
                    //{
                    //    if (string.IsNullOrEmpty(VerficaNull))
                    //        ListCpfVinculado.Remove(VerficaNull);
                    //}
                    //if (ListCpfVinculado.Count == 0)
                    //{
                    //    _repository.RetirarFilaSemCPF(solicitacao.NumeroInscricao);
                    //    continue;
                    //}
                    //foreach (var TestaCpf in ListCpfVinculado)
                    //{
                    driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[3]/input")).SendKeys(solicitacao.NumeroInscricao);
                    //driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[5]/input")).SendKeys(TestaCpf);
                    var CheckBox = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[7]/table/tbody/tr[1]/td[1]/span/input"));
                    if (CheckBox.Displayed)
                        CheckBox.Click();

                    solvedCaptchaCode = await SolveImagemCaptcha();
                    driver.ExecuteJavaScript($"document.getElementById('g-recaptcha-response-1').innerHTML='{solvedCaptchaCode}';");
                    var BtnBuscar = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[9]/button"));
                    if (BtnBuscar.Displayed)
                        BtnBuscar.Click();
                    //}
                    Thread.Sleep(20000);
                    var table = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/div[2]/table/tbody/tr[2]/td/div/table/tbody/tr[4]/td/table/tbody/tr[3]/td/table"));
                    IList<IWebElement> tableRow = table.FindElements(By.TagName("tr"));
                    IList<IWebElement> rowTD;

                    IMain check = new Main();
                    string? Compar = null;
                    List<Iptu> ObjList = new List<Iptu>();
                    foreach (IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        foreach (var title in rowTD)
                        {
                            Compar = title.Text;
                            bool valid = check.IsNumber(Compar);
                            if (valid)
                            {

                                //castin do objeto
                                Iptu Obj = new Iptu();
                                {
                                    Obj.Vencimento = rowTD[1].Text;
                                    Obj.Pagavel = rowTD[2].Text;
                                    Obj.Pagamento = rowTD[3].Text;
                                    Obj.VlParcela = rowTD[4].Text;
                                    Obj.Total = rowTD[5].Text;
                                    Obj.VlPago = rowTD[6].Text;
                                }
                                Obj.Titular = solicitacao.Titular;
                                Obj.NumeroInscricao = solicitacao.NumeroInscricao.Trim();
                                Obj.VlrVenal = solicitacao.VlrVenal;

                                if (!string.IsNullOrEmpty(Obj.Processo))
                                {
                                    Obj.Processo = Obj.Processo.Replace("/", "") + "8260606";
                                    if (Obj.Processo.Length < 19)
                                    {
                                        var DV = CalcularDV(Obj.Processo);
                                        Obj.Processo = "0" + Obj.Processo;
                                        var FaltForOnze = Obj.Processo.Length - 11;
                                        Obj.Processo = Obj.Processo.Substring(0, 7) + DV + Obj.Processo.Substring(FaltForOnze, 11);
                                    }
                                    Obj.Processo = Obj.Processo.Substring(0, 7) + "-" + Obj.Processo.Substring(7, 2) + "." + Obj.Processo.Substring(9, 4) + "." + Obj.Processo.Substring(13, 1) + "." + Obj.Processo.Substring(14, 2) + "." + Obj.Processo.Substring(16, 4);

                                }
                                ObjList.Add(Obj);
                                break;
                            }
                        }
                    }



                    driver.Url = "https://grp.suzano.sp.gov.br/portalcidadao/#78c3e513dd43cb27d8a3e2f376196ffc656d7ea577b2c6fb7a1c52bb7c9f265%C5%A44f09dc8471b15b9c26e8fa819ffb99e358e317d160499192fbd818832bd9d38dd9c75a51ff76ee066b1954822ae6f9e3f3ac1e6010ad93f0703bcb7a40b89f6268d077df8715226393630d16c115066908d71e41046bd68dcbcd64bfd2ffb54a28fd445d5a4d99c8e4356f2f00aabf70482deaf97895a848f0e8ca3179be62fd0944e495f662444456948c3cb844f8efdf474f644ffddaef555dacd1235c57ad1c9fa940932d08b6828284e55a768fc266f48d6c0409197c6f75a1b102454673ce4666dea838e4ef862a5b7144297b39";
                    Thread.Sleep(7000);




                    BtnImovel = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[1]/span[2]/input"));
                    if (BtnImovel.Displayed)
                        BtnImovel.Click();

                    driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[3]/input")).SendKeys(solicitacao.NumeroInscricao);

                    solvedCaptchaCode = await SolveImagemCaptcha();
                    driver.ExecuteJavaScript($"document.getElementById('g-recaptcha-response-2').innerHTML='{solvedCaptchaCode}';");
                    BtnBuscar = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/form/div/div[7]/button"));
                    if (BtnBuscar.Displayed)
                        BtnBuscar.Click();

                    Thread.Sleep(30000);
                    try
                    {
                        driver.SwitchTo().Alert().Accept();
                        continue;
                    }
                    catch { }

                    table = driver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div[2]/div/div[2]/table/tbody/tr[2]/td/div/table[2]/tbody/tr[2]/td/table/tbody/tr/td/table"));
                    tableRow = table.FindElements(By.TagName("tr"));
                    rowTD = null;

                    check = new Main();
                    Compar = null;
                    ObjList = new List<Iptu>();
                    foreach (IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        foreach (var title in rowTD)
                        {
                            Compar = title.Text;
                            var textoTrat = Compar.Split("\r\n");
                            bool valid = check.IsNumber(textoTrat[0]);
                            if (valid && textoTrat.Count() > 2)
                            {

                                //castin do objeto
                                Iptu Obj = new Iptu();
                                {
                                    Obj.Vencimento = textoTrat[0];
                                    Obj.Tipo = textoTrat[1];
                                    Obj.Status = textoTrat[2];
                                    Obj.VlParcela = textoTrat[3];
                                    Obj.Total = textoTrat[4];
                                }
                                Obj.Titular = solicitacao.Titular;
                                Obj.NumeroInscricao = solicitacao.NumeroInscricao.Trim();
                                Obj.VlrVenal = solicitacao.VlrVenal;
                                if (Obj.Status.Contains("EXE"))
                                {
                                    Obj.Processo = "000000000000000";
                                }
                                ObjList.Add(Obj);
                                break;
                            }
                        }

                    }


                    //chamada sql para registro no banco
                    if (ObjList.Count > 0)
                        _repository.InserirTextoTabelaFilaSuzaso(ObjList);

                    _repository.Processado(solicitacao.NumeroInscricao.Trim());
                    //driver.Close();
                    //driver.Quit();

                }
                MatarProcessos();
                return;
            }

            catch (Exception ex)
             {


                //Mata o chrome driver caso ele esteja aberto em segundo plano
                Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
                foreach (var chromeDriverProcess in chromeDriverProcesses)
                {

                    if (chromeDriverProcess.Id == id)
                    {
                        driver.Close();
                        driver.Dispose();
                        driver.Quit();
                        chromeDriverProcess.Kill();
                    }
                }
                return;
            }
        }

        private string downloadCaptcha(IWebDriver chromeDriver)
        {
            IWebElement? captchaImage = EsperarElemento(chromeDriver, "Id", "i29captchaimg");
            if (captchaImage == null)
                captchaImage = EsperarElemento(chromeDriver, "Id", "i20captchaimg");
            string Tick = DateTime.Now.Ticks.ToString();
            string img = GetElementScreenShott(driver, Tick);
            return img;
        }

        public static string GetElementScreenShott(IWebDriver driver, string tick)
        {
            try
            {
                Screenshot sc = ((ITakesScreenshot)driver).GetScreenshot();
                sc.SaveAsFile(@$"F:\AutomacaoIPTU\Captcha\{tick}.png");
                //sc.SaveAsFile(@$"C:\Users\FlávioSilvaVanquishC\Downloads\{tick}.png");
                tick = @$"F:\AutomacaoIPTU\Captcha\{tick}.png";
                //tick = @$"C:\Users\FlávioSilvaVanquishC\Downloads\{tick}.png";
                return tick;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IWebElement? EsperarElemento(IWebDriver? drive, string by, string element)
        {
            //Metodo responsavel por buscar elementos no site, caso não encontrar tentar 3x e tratar erro.
            IWebElement? Element = null;
            for (int NTentativas = 0; NTentativas < 3; NTentativas++)
            {
                try
                {
                    if (drive != null)
                    {
                        Thread.Sleep(100);
                        if (by.Equals("Id"))
                            Element = drive.FindElement(By.Id(element));
                        if (by.Equals("XPath"))
                            Element = drive.FindElement(By.XPath(element));

                        if (by.Equals("ClassName"))
                            Element = drive.FindElement(By.ClassName(element));
                        if (by.Equals("LinkText"))
                            Element = drive.FindElement(By.LinkText(element));
                        if (by.Equals("CssSelector"))
                            Element = drive.FindElement(By.CssSelector(element));
                        if (by.Equals("Name"))
                            Element = drive.FindElement(By.Name(element));
                        if (by.Equals(" TagName"))
                            Element = drive.FindElement(By.TagName(element));
                        if (by.Equals("PartialLinkText"))
                            Element = drive.FindElement(By.PartialLinkText(element));
                    }
                    return Element;
                }
                catch (Exception)
                {
                    Thread.Sleep(200);
                }
            }
            return null;
        }

        public async Task NavegacaoItaqua(List<Iptu> Solicitacao, IWebDriver? driver)
        {
            string? imagePath;
            Actions actions = new Actions(driver);
            try
            {

                foreach (var solicitacao in Solicitacao)
                {
                    if (solicitacao.NumeroInscricao != null)
                        solicitacao.NumeroInscricao = solicitacao.NumeroInscricao.Trim();
                    List<string> ListCpfVinculado = new List<string>();
                    List<string> ListCpfVinculadoRetirar = new List<string>();
                    //Logica de Busca Por cpfs
                    //var RetirarFila = _repository.RetornaExcluido(solicitacao.CodigoLote.Trim());
                    //if (RetirarFila)
                    //{
                    //    _repository.RetirarFilaSemCPF(solicitacao.NumeroInscricao);
                    //    continue;
                    //}

                    var LisCPF = _repository.RetornaCPF(solicitacao.CodigoLoteamentoLoteador.Trim());
                    ListCpfVinculado.AddRange(_repository.RetornaCPFVinculado(solicitacao.CodigoLote.Trim()));
                    if (LisCPF.Count > 0)
                    {
                        foreach (var Tentativas in LisCPF)
                        {
                            var ret = _repository.RetornaCPFPorLoteador(Tentativas);
                            if (ret.Trim() != null)
                                ListCpfVinculado.Add(ret);
                        }
                    }
                    foreach (var VerficaNull in ListCpfVinculado)
                    {
                        if (string.IsNullOrEmpty(VerficaNull))
                            ListCpfVinculadoRetirar.Add(VerficaNull);
                    }
                    foreach (var VerficaNull in ListCpfVinculadoRetirar)
                    {
                        if (string.IsNullOrEmpty(VerficaNull))
                            ListCpfVinculado.Remove(VerficaNull);
                    }
                    ListCpfVinculado.Add("043.051.218-08");
                    ListCpfVinculado.Add("939.274.608-34");

                    driver.Url = "https://itaquaquecetuba.siltecnologia.com.br/loginWeb.jsp?execobj=ServicosWebSite";
                    Thread.Sleep(4000);
                    var BtnImovel = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[1]/div/div/div[1]/div[2]"));
                    if (BtnImovel.Displayed)
                        BtnImovel.Click();
                    Thread.Sleep(4000);

                    IWebElement? Verifc = null;
                    Captchas? solvedCaptchaCode = null;
                    imagePath = null;

                    foreach (var TestaCPF in ListCpfVinculado)
                    {
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[5]/input")).Clear();
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[3]/input")).Clear();
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[2]/input")).Clear();

                        imagePath = downloadCaptcha(driver);
                        solvedCaptchaCode = await SolveCaptcha(imagePath);
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[2]/input")).SendKeys(solicitacao.NumeroInscricao);
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[3]/input")).SendKeys(TestaCPF);
                        driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[5]/input")).SendKeys(solvedCaptchaCode.Code);
                        var BtnConsultar = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[6]/div/button[1]"));
                        if (BtnConsultar.Displayed)
                            BtnConsultar.Click();
                        Thread.Sleep(600);
                        solicitacao.Cpf = TestaCPF;
                        Verifc = EsperarElemento(driver, "XPath", "/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[1]/div/div");
                        if (Verifc == null)
                            break;
                        if (Verifc.Text.Contains("Texto da imagem incorreto."))
                        {
                            var Contest = await ContestCaptcha(solvedCaptchaCode.Id);

                            imagePath = downloadCaptcha(driver);
                            solvedCaptchaCode = await SolveCaptcha(imagePath);
                            driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[5]/input")).Clear();
                            driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[5]/input")).SendKeys(solvedCaptchaCode.Code);
                            Thread.Sleep(400);
                            BtnConsultar = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[6]/div/button[1]"));
                            BtnConsultar.Click();
                            Thread.Sleep(4000);
                            solicitacao.Cpf = TestaCPF;
                            Verifc = EsperarElemento(driver, "XPath", "/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[2]/div[3]/div[2]/div[1]/div/div");
                            if (Verifc == null)
                                break;
                        }
                    }
                    if (Verifc == null)
                    {
                        _repository.InserirCPF(solicitacao.NumeroInscricao, solicitacao.Cpf.Trim());
                        var table = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div[4]/div[2]/div/div[1]/div/div/table"));
                        IList<IWebElement> tableRow = table.FindElements(By.TagName("tr"));
                        IList<IWebElement> rowTD;
                        IMain check = new Main();
                        string? Compar = null;
                        List<Iptu> ObjList = new List<Iptu>();
                        foreach (IWebElement row in tableRow)
                        {
                            rowTD = row.FindElements(By.TagName("td"));
                            foreach (var title in rowTD)
                            {
                                Compar = title.Text;
                                if (Compar.Equals("IPTU"))
                                {
                                    //castin do objeto
                                    Iptu Obj = new Iptu();
                                    {
                                        Obj.Tipo = rowTD[1].Text;
                                        Obj.Ano = rowTD[2].Text;
                                        Obj.Original = rowTD[3].Text;
                                        Obj.Correcao = rowTD[4].Text;
                                        Obj.Juros = rowTD[5].Text;
                                        Obj.Multa = rowTD[6].Text;
                                        Obj.SubTotal = rowTD[7].Text;
                                        Obj.Honorarios = rowTD[8].Text;
                                        Obj.Total = rowTD[9].Text;
                                        Obj.Desconto = rowTD[10].Text;
                                        Obj.CDA = rowTD[11].Text;
                                        Obj.Processo = rowTD[12].Text;
                                    }
                                    Obj.Titular = solicitacao.Titular;
                                    Obj.NumeroInscricao = solicitacao.NumeroInscricao.Trim();
                                    Obj.VlrVenal = solicitacao.VlrVenal;

                                    if (!string.IsNullOrEmpty(Obj.Processo))
                                    {
                                        Obj.Processo = Obj.Processo.Replace("/", "") + "8260278";
                                        if (Obj.Processo.Length < 19)
                                        {
                                            var DV = CalcularDV(Obj.Processo);
                                            Obj.Processo = "0" + Obj.Processo;
                                            var FaltForOnze = Obj.Processo.Length - 11;
                                            Obj.Processo = Obj.Processo.Substring(0, 7) + DV + Obj.Processo.Substring(FaltForOnze, 11);
                                        }
                                        Obj.Processo = Obj.Processo.Substring(0, 7) + "-" + Obj.Processo.Substring(7, 2) + "." + Obj.Processo.Substring(9, 4) + "." + Obj.Processo.Substring(13, 1) + "." + Obj.Processo.Substring(14, 2) + "." + Obj.Processo.Substring(16, 4);

                                    }
                                    ObjList.Add(Obj);
                                    break;
                                }
                            }
                        }
                        //chamada sql para registro no banco
                        if (ObjList.Count > 0)
                            _repository.InserirTextoTabelaFila(ObjList);
                    }
                    else
                    {
                        if (Verifc.Text.Contains("Texto da imagem incorreto."))
                        {
                            var Contest = await ContestCaptcha(solvedCaptchaCode.Id);
                        }
                        else
                            _repository.InserirSemDados(solicitacao.NumeroInscricao);
                        continue;
                    }
                    Thread.Sleep(1000);
                    driver.Url = "https://itaquaquecetuba.siltecnologia.com.br/loginWeb.jsp?execobj=ServicosWebSite";
                    solvedCaptchaCode = null;
                    imagePath = null;
                    Thread.Sleep(1000);
                    var BtnVenal = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[1]/div/div/div[1]/div[7]/div[2]/img"));
                    if (BtnVenal.Displayed)
                        BtnVenal.Click();

                    Thread.Sleep(1000);


                    imagePath = downloadCaptcha(driver);
                    solvedCaptchaCode = await SolveCaptcha(imagePath);

                    driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[11]/div[3]/div[2]/div[2]/input")).SendKeys(solicitacao.NumeroInscricao);
                    driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[11]/div[3]/div[2]/div[3]/input")).SendKeys(solicitacao.Cpf);
                    driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[11]/div[3]/div[2]/div[5]/input")).SendKeys(solvedCaptchaCode.Code);

                    var BtnGerar = driver.FindElement(By.XPath("/html/body/div[2]/div/form/section[2]/div/div/div[2]/div/div/div[11]/div[3]/div[2]/div[6]/div/button"));
                    if (BtnGerar.Displayed)
                        BtnGerar.Click();

                    Thread.Sleep(8000);
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    Thread.Sleep(800);
                    driver.Manage().Window.Maximize();
                    Thread.Sleep(1000);
                    try
                    {
                        using (var client = new WebClient())
                        {
                            byte[] data = client.DownloadData(driver.Url);
                            using (var pdf = new BitMiracle.Docotic.Pdf.PdfDocument(data))
                            {
                                string text = pdf.GetText();
                                var textoTrat = text.Split("\r\n");
                                string valorvem;
                                foreach (var trat in textoTrat)
                                {
                                    if (trat.Contains("Valor Venal Imóvel"))
                                    {
                                        valorvem = trat.Substring(36, 9);
                                        _repository.InserirValorVenal(solicitacao.NumeroInscricao.Trim(), valorvem);
                                        _repository.Processado(solicitacao.NumeroInscricao.Trim());
                                        solicitacao.VlrVenal = valorvem;
                                    }
                                }

                            }
                        }
                        driver.Close();
                        Thread.Sleep(1000);
                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(solicitacao.VlrVenal))
                    {
                        IList<string> tabs = new List<string>(driver.WindowHandles);
                        driver.SwitchTo().Window(tabs[0]);
                        Thread.Sleep(2000);
                    }
                }
                MatarProcessos();
                return;
            }
            catch (Exception ex)
            {

                if (ex.ToString().Contains("ERROR_CAPTCHA_UNSOLVABLE"))
                    _repository.AlterarProcessando();
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var ticks = DateTime.Now.Ticks;
                screenshot.SaveAsFile(@$"F:\AutomacaoIPTU\ScreenShot\{ticks}.jpg");
                //screenshot.SaveAsFile(@$"C:\Users\FlávioSilvaVanquishC\Downloads\{ticks}.jpg");
                Solicitacao.First().Log_Erro = ex.Message;
                MatarProcessos();
                return;
            }
        }
    }
}
