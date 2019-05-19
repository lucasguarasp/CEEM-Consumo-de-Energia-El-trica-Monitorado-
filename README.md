# CEEM - Consumo de Energia Elétrica Monitorado

O que é?
---
CEEM é um sistema para monitoramento de energia elétrica (Consumo de Energia Elétrica Monitorada).

Como funciona?
---
O sistema permite ao usuário visualizar a partir de uma página web, o quanto uma casa, cômodo está consumindo de energia.
 
Sobre o equipamento:
---
O equipamento é composto por sensores de corrente e tensão não invasivo, que fazem o input na placa arduino que por sua vez, a partir de uma conexão com a internet, realizará envios em intervalos de 5 minutos com a informação da média de consumo atual do ponto.
 
Como é realizada a coleta dos dados pelo equipamento?
---
A coleta de dados é realizada a partir de um equipamento instalado na rede elétrica que enviará informações para alimentação da base de dados.
 
Como é realizado o tratamento dos dados pelo equipamento?
---
O equipamento ficará recebendo diversas informações sobre a corrente e tensão dos determinados sensores, após o tempo de 5 minutos, equipamento gerará um arquivo .txt contendo a média dos sensor e o código do equipamento e armazenará temporariamente na memória do equipamento.
Média dos valores: (Soma dos valores / Quantidade de Informações).
 
Como é realizado o envio dos dados pelo equipamento?
---
O equipamento realizará o envio do arquivo para o servidor, caso o envio ocorra conforme o esperado, o arquivo .txt deverá ser excluído da memória do equipamento.
 
Como é realizado a inserção dos dados lidos pelos equipamentos na base de dados?
---
Haverá um serviço que realizará o processamento dos arquivos enviados para o sistema, após a leitura, o serviço converterá as informações para a classe correspondente e armazenará a informação na base dados.
 
Como o sistema disponibilizará as informações de consumo para o usuário?
---
Quanto o usuário realizar a autenticação, o sistema deverá realizar uma busca contendo a informação de consumo mais recente existente na base de dados de todos os sensores registrados para o usuário x equipamento registrado x sensores registrados.
O usuário poderá realizar buscas e comparações de datas ou horários específicos de quaisquer sensores registrados para sua conta.
O sistema também conterá categorias de sensor como por exemplo: sala de estar, cozinha, quarto.
 
Relatórios e visualizações
---
O sistema permitirá o usuário realizar diversos tipos de consulta dos gastos de energia de cada ponto e do local onde foi instalado, também será possível gerar relatórios e gráficos dos sensores monitorados e fazer comparações entre os mesmos.
O usuário que tiver sensor cadastrado com categorias predefinidas poderá verificar a média de outros usuários do sistema, sendo assim, o usuário terá uma noção de se seu consumo de energia em determinado ponto está na acima, abaixo ou na média de consumo geral dos usuários do sistema.
 
Sobre a monetização
---
O aparelho será vendido para os usuários do sistema e o acesso ao sistema será cobrado mensalmente, sendo assim, os usuários inadimplentes terão o acesso ao sistema interrompido porém, caso já tenha pago o aparelho, poderá continuar com o mesmo na residência.
