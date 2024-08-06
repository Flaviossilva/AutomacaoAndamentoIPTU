using AutomacaoIPTU.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace AutomacaoIPTU.Business
{

    internal class Business
    {
        Service.Service _service = new();
        private static readonly Repository.Repository repository = new();
        Repository.Repository _repository = repository;
        List<Iptu> _solicitacao = new();
        IWebDriver? driver;
        string? ComparaMunicipio;


        public async Task Start()
        {
            int id = 0;
            #region Inicializadores

            #endregion
            try
            {
              
                _service.MatarProcessos();          
                _repository.AlimentarFila();
                _repository.AlimentcarFilaSeguinte();
                _repository.ArrumarDatas();
                _service.ArrumarProcessos();
                _repository.AlterarSemCPF();
                _repository.AlterarProcessando();
                _service.GerarCodigoMunicipios();
                _solicitacao = _repository.RetornaSolicitacoes();
                while (_solicitacao.Count > 0)
                {
                    driver = _service.IniciarChrome();
                 
                    ComparaMunicipio = _solicitacao[0].Cidade.Substring(0, 3).ToUpper();
                    foreach (var AtualizaProcessamento in _solicitacao)
                    {
                        _repository.AlterarConsultados(AtualizaProcessamento);
                    }
                    //Busca lista de Navegações, e seleciona a navegação baseado no ComparaMunicipio
                    var servicosPorMunicipio = _service.RetornoLista(_solicitacao);
                    if (servicosPorMunicipio.TryGetValue(ComparaMunicipio, out var servico))
                        await servico(_solicitacao, driver);

                    _solicitacao.Clear();
                    _service.MatarProcessos();
                    _solicitacao = _repository.RetornaSolicitacoes();
                }
                //volta para pendente os casos que ficaram presos em processando
                _repository.AlterarProcessando();
                //Logica para Reprocessar casos com erros e parados em processamento
                _solicitacao = _repository.RetornaSolicitacoes();
                if (_solicitacao.Count > 0)
                    await Start();
            }
            catch (Exception ex)
            {
                Restart();
                throw;
            }
        }


        public void MatarId(ChromeDriver driver,int id)
        {
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {

                if (chromeDriverProcess.Id == id)
                {
                    driver.Dispose();
                    chromeDriverProcess.Kill();
                }
            }
        }
        public void Stop()
        {
            //Mata o chrome driver caso ele esteja aberto em segundo plano
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }
            Process[] chrome = Process.GetProcessesByName("chrome");
            foreach (var chromeDriverProcess in chrome)
            {
                chromeDriverProcess.Kill();
            }
            Process.GetCurrentProcess().Kill();
        }

        public async void Restart()
        {
            Thread.Sleep(4000);
            var task = Start();
            await Task.WhenAll(task);
        }

        public async Task StartThreads(int numberOfThreads)
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numberOfThreads; i++)
            {
                Thread.Sleep(5000);
                tasks.Add(Task.Run(() => Start()));
            }

            await Task.WhenAll(tasks);
        }
    }
}












//internal class Business
//{
//    Service.Service _service = new();
//    Repository.Repository _repository = new();
//    List<Iptu> _solicitacao = new();
//    IWebDriver? driver;
//    string? ComparaMunicipio;

//    public async Task Start()
//    {
//        #region Inicializadores

//        #endregion

//        try
//        {
//            _service.MatarProcessos();
//            _repository.AlimentarFila();
//            _service.ArrumarProcessos();
//            _repository.AlterarSemCPF();
//            _repository.AlterarProcessando();
//            _service.GerarCodigoMunicipios();
//            _solicitacao = _repository.RetornaSolicitacoes();
//            while (_solicitacao.Count > 0)
//            {
//                driver = _service.IniciarChrome();
//                ComparaMunicipio = _solicitacao[0].Cidade.Substring(0, 3).ToUpper();
//                foreach (var AtualizaProcessamento in _solicitacao)
//                {
//                    _repository.AlterarConsultados(AtualizaProcessamento);
//                }
//                //Busca lista de Navegações, e seleciona a navegação baseado no ComparaMunicipio
//                var servicosPorMunicipio = _service.RetornoLista(_solicitacao);
//                if (servicosPorMunicipio.TryGetValue(ComparaMunicipio, out var servico))
//                    await servico(_solicitacao, driver);

//                _solicitacao.Clear();
//                _solicitacao = _repository.RetornaSolicitacoes();
//            }
//            //volta para pendente os casos que ficaram presos em processando
//            _repository.AlterarProcessando();
//            //Logica para Reprocessar casos com erros e parados em processamento
//            _solicitacao = _repository.RetornaSolicitacoes();
//            if (_solicitacao.Count > 0)
//                await Start();
//        }
//        catch (Exception ex)
//        {
//            Restart();
//            throw;
//        }
//    }
//    public void Stop()
//    {
//        //Mata o chrome driver caso ele esteja aberto em segundo plano
//        Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
//        foreach (var chromeDriverProcess in chromeDriverProcesses)
//        {
//            chromeDriverProcess.Kill();
//        }
//        Process[] chrome = Process.GetProcessesByName("chrome");
//        foreach (var chromeDriverProcess in chrome)
//        {
//            chromeDriverProcess.Kill();
//        }
//        Process.GetCurrentProcess().Kill();
//    }

//    public void Restart()
//    {
//        //Mata o chrome driver caso ele esteja aberto em segundo plano
//        Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
//        foreach (var chromeDriverProcess in chromeDriverProcesses)
//        {
//            chromeDriverProcess.Kill();
//        }
//        Process[] chrome = Process.GetProcessesByName("chrome");
//        foreach (var chromeDriverProcess in chrome)
//        {
//            chromeDriverProcess.Kill();
//        }
//        Thread.Sleep(900);
//        Task task = Start();
//    }
//}
//}
