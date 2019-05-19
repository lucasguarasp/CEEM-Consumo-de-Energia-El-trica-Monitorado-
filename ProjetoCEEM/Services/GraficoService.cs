using ProjetoCEEM.Models;
using ProjetoCEEM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Services
{
    public class GraficoService
    {
        /// <summary>
        /// Metodo responsável por gerar o gráfico de Barras Verticais do ano informado para um determinado usuário.
        /// </summary>
        /// <param name="Ano">Ano do qual deseja extrair o gráfico</param>
        /// <param name="EquipamentoId">Equipamento que realizou a coleta dos dados desejados</param>
        /// <param name="db">Instancia da classe Context</param>
        /// <returns>Retorna uma string pronta para geração de um grafico de barras verticais com o HighCharts</returns>
        public static string GetGraficoBarrasVerticais(int Ano, int EquipamentoId, Context db)
        {
            var grafico = "";
            var graficoViewModel = GetModelGraficoAnual(Ano, EquipamentoId, db);
            foreach (var item in graficoViewModel)
            {
                grafico += string.Concat(GetSerieBarraAnual(item), ",");
            }
            grafico = grafico.Substring(0, grafico.Length - 1);
            return grafico;
        }
        /// <summary>
        /// Retorna um gráfico de Pizza de um determinado equipamento
        /// </summary>
        /// <param name="Mes"></param>
        /// <param name="Ano"></param>
        /// <param name="EquipamentoId"></param>
        /// <param name="db"></param>
        /// <returns>Retorna uma string pronta para geração de um grafico de pizza com o HighCharts</returns>
        public static string GetGraficoPizza(int Mes, int Ano, int EquipamentoId, Context db)
        {
            var grafico = "";
            var graficoViewModel = GetModelGraficoPizza(Mes, Ano, EquipamentoId, db);
            foreach (var item in graficoViewModel)
            {
                grafico += string.Concat(GetSeriePizza(item), ",");
            }
            grafico = grafico.Substring(0, grafico.Length - 1);
            return grafico;
        }
        /// <summary>
        /// Retorna um gráfico de Pizza de um determinado equipamento
        /// </summary>
        /// <param name="Dia">Dia desejado para gerar o grafico</param>
        /// <param name="Mes">Mes desejado para gerar o grafico</param>
        /// <param name="Ano">Ano desejado para gerar o grafico</param>
        /// <param name="EquipamentoId">Equipamento que realizou a coleta dos dados desejados</param>
        /// <param name="db">Instancia da classe Context</param>
        /// <returns>Retorna uma string pronta para geração de um grafico de pizza com o HighCharts</returns>
        public static string GetGraficoPizza(int Dia, int Mes, int Ano, int EquipamentoId, Context db)
        {
            var grafico = "";
            var graficoViewModel = GetModelGraficoPizza(Dia, Mes, Ano, EquipamentoId, db);
            foreach (var item in graficoViewModel)
            {
                grafico += string.Concat(GetSeriePizza(item), ",");
            }
            grafico = grafico.Substring(0, grafico.Length - 1);
            return grafico;
        }
        #region Logica de Criação dos Gráficos
        private static string GetSerieBarraAnual(GraficoBarrasVerticaisViewModel grafico)
        {
            string graficoString = string.Format("{0} name: '{1}', data: [{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}] {2}"
                , "{"
                , grafico.LocalSensor
                , "}"
                , grafico.KwAnual.ElementAt(0).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(1).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(2).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(3).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(4).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(5).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(6).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(7).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(8).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(9).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(10).ToString().Replace(",", ".")
                , grafico.KwAnual.ElementAt(11).ToString().Replace(",", ".")
                );
            return graficoString;
        }

        private static string GetSeriePizza(GraficoPizzaViewModel grafico)
        {
            var porcentagemKW = grafico.PorcentagemKW.ToString().Replace(",", ".");
            string graficoString = string.Format("{0} name: '{1}', y: {3} {2}"
                , "{"
                , grafico.LocalSensor
                , "}"
                , porcentagemKW
                );
            return graficoString;
        }
        private static List<GraficoBarrasVerticaisViewModel> GetModelGraficoAnual(int Ano, int EquipamentoId, Context db)
        {
            var grafico = new List<GraficoBarrasVerticaisViewModel>();
            var equipamento = db.Equipamentos.Find(EquipamentoId);
            var sensores = GetSensores(equipamento.EquipamentoId, db);
            foreach (var item in sensores)
            {
                var sensor = GetSensor(item.SensorId, EquipamentoId, db);
                var consumoAnual = GetConsumoBarraVertical(Ano, item.SensorId, item.EquipamentoId, db);

                var graficoSensor = new GraficoBarrasVerticaisViewModel
                {
                    EquipamentoId = equipamento.EquipamentoId,
                    Ano = Ano,
                    LocalEquipamento = equipamento.Local,
                    LocalSensor = item.Local,
                    SensorId = item.SensorId,
                    KwAnual = consumoAnual
                };
                grafico.Add(graficoSensor);
            }
            return grafico;
        }

        private static List<GraficoPizzaViewModel> GetModelGraficoPizza(int Mes, int Ano, int EquipamentoId, Context db)
        {
            var grafico = new List<GraficoPizzaViewModel>();
            var equipamento = db.Equipamentos.Find(EquipamentoId);
            var sensores = GetSensores(equipamento.EquipamentoId, db);
            var consumos = GetConsumoMensal(Mes, Ano, EquipamentoId, db);
            var consumoTotal = SomaConsumos(consumos);
            foreach (var item in sensores)
            {
                var sensor = GetSensor(item.SensorId, EquipamentoId, db);
                var consumoMensal = GetConsumoMensal(Mes, Ano, item.SensorId, item.EquipamentoId, db);
                var consumoMensalSensor = SomaConsumos(consumoMensal);
                var porcentagemKw = (consumoMensalSensor / consumoTotal) * 100;
                var graficoSensor = new GraficoPizzaViewModel
                {
                    EquipamentoId = equipamento.EquipamentoId,
                    Ano = Ano,
                    LocalEquipamento = equipamento.Local,
                    LocalSensor = item.Local,
                    SensorId = item.SensorId,
                    PorcentagemKW = porcentagemKw,
                    Mes = Mes
                };
                grafico.Add(graficoSensor);
            }
            return grafico;
        }

        private static List<GraficoPizzaViewModel> GetModelGraficoPizza(int Dia, int Mes, int Ano, int EquipamentoId, Context db)
        {
            var grafico = new List<GraficoPizzaViewModel>();
            var equipamento = db.Equipamentos.Find(EquipamentoId);
            var sensores = GetSensores(equipamento.EquipamentoId, db);
            var consumos = GetConsumoDiaAtual(Dia, Mes, Ano, EquipamentoId, db);
            var consumoTotal = SomaConsumos(consumos);
            foreach (var item in sensores)
            {
                var sensor = GetSensor(item.SensorId, EquipamentoId, db);
                var consumoMensal = GetConsumoDiaAtual(Dia, Mes, Ano, item.SensorId, item.EquipamentoId, db);
                var consumoMensalSensor = SomaConsumos(consumoMensal);
                var porcentagemKw = (consumoMensalSensor / consumoTotal) * 100;
                var graficoSensor = new GraficoPizzaViewModel
                {
                    EquipamentoId = equipamento.EquipamentoId,
                    Ano = Ano,
                    LocalEquipamento = equipamento.Local,
                    LocalSensor = item.Local,
                    SensorId = item.SensorId,
                    PorcentagemKW = porcentagemKw,
                    Mes = Mes
                };
                grafico.Add(graficoSensor);
            }
            return grafico;
        }

        private static List<Sensor> GetSensores(int EquipamentoId, Context db)
        {
            var sensor = db.Sensores.Where(s => s.EquipamentoId == EquipamentoId).ToList();
            return sensor;
        }
        private static Sensor GetSensor(int SensorId, int EquipamentoId, Context db)
        {
            var sensor = db.Sensores.Find(SensorId, EquipamentoId);
            return sensor;
        }
        /// <summary>
        /// Retorna o consumo mensal de um determinado Sensor de um determinado Equipamento
        /// </summary>
        /// <param name="Mes"></param>
        /// <param name="Ano"></param>
        /// <param name="SensorId"></param>
        /// <param name="EquipamentoId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private static List<Consumo> GetConsumoMensal(int Mes, int Ano, int SensorId, int EquipamentoId, Context db)
        {
            var consumos = db.Consumos.Where(c => c.DataMedida.Month == Mes && c.DataMedida.Year == Ano && c.SensorId == SensorId && c.EquipamentoId == EquipamentoId).ToList();
            return consumos;
        }
        /// <summary>
        /// Retorna o consumo mensal de todos os sensores de um determinado equipamento
        /// </summary>
        /// <param name="Mes"></param>
        /// <param name="Ano"></param>
        /// <param name="EquipamentoId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private static List<Consumo> GetConsumoMensal(int Mes, int Ano, int EquipamentoId, Context db)
        {
            var consumos = db.Consumos.Where(c => c.DataMedida.Month == Mes && c.DataMedida.Year == Ano && c.EquipamentoId == EquipamentoId).ToList();
            return consumos;
        }

        /// <summary>
        /// Retorna todos os consumos do dia atual de um determinado sensor de um determinado equipamento
        /// </summary>
        /// <param name="Dia"></param>
        /// <param name="Mes"></param>
        /// <param name="Ano"></param>
        /// <param name="SensorId"></param>
        /// <param name="EquipamentoId"></param>
        /// <param name="db"></param>
        /// <returns>Retorna uma lista de Consumo para um determinado Sensor de um determinado Equipamento</returns>
        private static List<Consumo> GetConsumoDiaAtual(int Dia, int Mes, int Ano, int SensorId, int EquipamentoId, Context db)
        {
            var consumos = db.Consumos.Where(c => c.DataMedida.Day == Dia && c.DataMedida.Month == Mes && c.DataMedida.Year == Ano && c.SensorId == SensorId && c.EquipamentoId == EquipamentoId).ToList();
            return consumos;
        }
        /// <summary>
        /// Retorna todos os consumos do dia atual de um determinado equipamento
        /// </summary>
        /// <param name="Dia"></param>
        /// <param name="Mes"></param>
        /// <param name="Ano"></param>
        /// <param name="EquipamentoId"></param>
        /// <param name="db"></param>
        /// <returns>Retorna uma lista de Consumo para um determinado Equipamento</returns>
        private static List<Consumo> GetConsumoDiaAtual(int Dia, int Mes, int Ano, int EquipamentoId, Context db)
        {
            var consumos = db.Consumos.Where(c => c.DataMedida.Day == Dia && c.DataMedida.Month == Mes && c.DataMedida.Year == Ano && c.EquipamentoId == EquipamentoId).ToList();
            return consumos;
        }

        private static List<double> GetConsumoBarraVertical(int Ano, int SensorId, int EquipamentoId, Context db)
        {
            var consumoAnual = new List<double>();
            for (int i = 1; i <= 12; i++)
            {
                var consumoMensal = GetConsumoMensal(i, Ano, SensorId, EquipamentoId, db);
                var valorConsumoMensalKwh = SomaConsumos(consumoMensal);
                consumoAnual.Add(valorConsumoMensalKwh);
            }
            return consumoAnual;
        }

        private static double SomaConsumos(List<Consumo> Consumos)
        {
            double soma = 0;
            foreach (var item in Consumos)
            {
                soma += item.Kwh;
            }
            return soma;
        }
        #endregion
    }
}