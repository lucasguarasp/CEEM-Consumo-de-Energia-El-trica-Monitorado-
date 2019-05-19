namespace ProjetoCEEM.Migrations
{
    using ProjetoCEEM.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProjetoCEEM.Models.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ProjetoCEEM.Models.Context context)
        {
            //This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.E.g.

            context.StatusUsuarios.AddOrUpdate(
              s => s.Descricao,
              new StatusUsuario { Descricao = "Ativo" },
              new StatusUsuario { Descricao = "Inativo" },
              new StatusUsuario { Descricao = "Bloqueado" }
            );

            context.StatusOrdemServicos.AddOrUpdate(
                s => s.Descricao,
                new StatusOrdemServico { Descricao = "Aguardando Atendimento" },
                new StatusOrdemServico { Descricao = "Em Atendimento" },
                new StatusOrdemServico { Descricao = "Concluído" },
                new StatusOrdemServico { Descricao = "Cancelado" }
            );

            context.TipoContatos.AddOrUpdate(
                t => t.Descricao,
                new TipoContato { Descricao = "Email" },
                new TipoContato { Descricao = "Telefone" },
                new TipoContato { Descricao = "Celular" }
            );

            context.StatusCadastros.AddOrUpdate(
                c => c.Descricao,
                new StatusCadastro { Descricao = "Pendente" },
                new StatusCadastro { Descricao = "Concluído" },
                new StatusCadastro { Descricao = "Cancelado" }
            );

            context.PerguntaSegurancas.AddOrUpdate(
                c => c.Descricao,
                new PerguntaSeguranca { Descricao = "Qual o nome do seu pai ?" },
                new PerguntaSeguranca { Descricao = "Qual o nome de sua mãe ?" },
                new PerguntaSeguranca { Descricao = "Qual o nome do seu bicho de estimação ?" },
                new PerguntaSeguranca { Descricao = "Qual seu esporte favorito ?" }
            );
            context.Perfis.AddOrUpdate(
                p => p.Descricao,
                new Perfil { Descricao = "Comum" },
                new Perfil { Descricao = "Administrador" },
                new Perfil { Descricao = "Desenvolvedor" }
            );

            context.CidadeAtendidas.AddOrUpdate(
                p => p.Cidade,
                    new CidadeAtendida { Cidade = "Lorena", Uf = "SP" },
                    new CidadeAtendida { Cidade = "Guaratinguetá", Uf = "SP" },
                    new CidadeAtendida { Cidade = "Canas", Uf = "SP" },
                    new CidadeAtendida { Cidade = "Taubaté", Uf = "SP" },
                    new CidadeAtendida { Cidade = "Pindamonhangaba", Uf = "SP" }
            );

            context.Bandeiras.AddOrUpdate(
                b => b.Cor,
                    new Bandeira { Cor = "Verde", Valor = 0, Descricao = "Não ha alteração no valor", BandeiraVigente = true },
                    new Bandeira { Cor = "Amarela", Valor = 1, Descricao = "a cada 100kWh" },
                    new Bandeira { Cor = "Vermelha Patamar 1", Valor = 3, Descricao = "a cada 100kWh" },
                    new Bandeira { Cor = "Vermelha Patamar 2", Valor = 5, Descricao = "a cada 100kWh" }
            );

            //http://www.aneel.gov.br/ranking-das-tarifas
            context.TaxaTarifarias.AddOrUpdate(
                t => t.Distribuidora,
                    new TaxaTarifaria { Distribuidora = "Eletropaulo", Uf = "SP", Valor = 0.42 },
                    new TaxaTarifaria { Valor = 0.487, Distribuidora = "Bandeirante", Uf = "SP" }
            );
        }
    }
}
