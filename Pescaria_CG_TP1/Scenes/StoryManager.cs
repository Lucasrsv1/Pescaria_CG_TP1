///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Scenes
///   Class:			StoryManager
///   Description:		Shows the current level's story.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using NAudio.Wave;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Prefabs;
using SharpGL;
using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Scenes {
	public class StoryManager : IScene {
		private static bool keepPlaying;
		private static readonly WaveOutEvent OutputDevice = new WaveOutEvent();
		private static readonly AudioFileReader audioFile = new AudioFileReader("./Audio/Story.mp3");

		private static bool texturesRegistered = false;
		private static readonly int SPACE = 10;
		private static readonly int[] idleDurations = new int[] { 12000, 8000, 8000, 13000, 15000, 6000, 15000, 8000, 12000, 8000 };
		private static readonly string[] quotes = new string[] {
			"\"Nós aprendemos com nossos pais que são os indivíduos que determinam seu próprio futuro, isto é, principalmente sua ambição e trabalho duro que determina seu destino na vida.\"\n- Ronald Reagan",
			"\"O homem não é livre a não ser que o Estado seja limitado.\nÀ medida que o governo expande, a liberdade contrai.\"\n- Ronald Reagan",
			"\"O governo deveria fazer apenas aquelas coisas que um indivíduo não pode fazer por conta própria.\"\n- Abraham Lincoln",
			"\"O capitalismo foi chamado de sistema da ganância. No entanto, é o sistema que elevou o padrão de vida de seus cidadãos mais pobres a um nível que nenhum sistema coletivista ou grupo tribal poderia jamais conceber.\"\n- Ayn Rand",
			"\"A felicidade não está relacionada ao materialismo ou a redistribuição de riqueza pelo governo. Ela está relacionada com o indivíduo definindo e buscando seus objetivos na vida. A promessa do livre mercado é permitir que o indivíduo conquiste seu próprio sucesso, buscando sua felicidade.\"\n- Arthur Brooks",
			"\"Um homem honesto é aquele que sabe que não pode consumir mais do que produziu.\"\n- Ayn Rand",
			"\"Desigualdade não é pobreza: combater a desigualdade não acaba com a pobreza e diminuir a pobreza não implica acabar com a desigualdade. É imprescindível separar esses dois conceitos para não sermos enganados pelos defensores do igualitarismo, os que querem apenas redistribuir a pobreza.\"\n- Mises Brasil",
			"\"O Capitalismo ensina as pessoas a trabalharem mais.\nJá o socialismo ensina as pessoas a pedirem mais.\"\n- Dennis Prager",
			"\"O inerente vício do capitalismo é o inigualitário compartilhamento de bençãos, já a inerente virtude do socialismo é o igualitário compartilhamento de misérias.\"\n- Winston Churchill",
			"\"O problema com o socialismo é que, cedo ou tarde, o dinheiro dos outros acaba.\"\n- Margaret Thatcher"
		};

		private static readonly int[] levelGoal = new int[] { 2000, 4000, 8500, 8500, 8500, 6500, 4000, 2000 };
		private static readonly string[][] speeches = new string[][] {
			new string[] {
				// LEVEL 1
				"Governador|Olá, eu sou o governador do Porto dos Peixes. Seja bem-vindo ao nosso maravilhoso estado!",
				"Bob Fisher|Muito obrigado! Eu sou Bob Fisher e vim para cá com grandes sonhos e planos! Ouvi dizer que vocês têm águas com uma vasta variedade de peixes.",
				"Governador|Sem dúvidas! Nosso mar tem muitos peixes, porém precisamos de alguém com interesse em explorar nossas águas.",
				"Bob Fisher|Pois então estou no lugar certo! Trabalhei por muitos anos com pesca e pretendo montar um negócio para explorar a riqueza deste mar e trazer empregos e prosperidade ao estado!",
				"Banqueiro|É muito bom ouvir isso! Prazer, eu sou o dono do Banco do Porto dos Peixes, e gostaria de lhe propor uma ajuda nessa empreitada!",
				"Bob Fisher|Pois tal ajuda será muito bem-vinda, Sr. Banqueiro. Embora tenha experiência com pesca, tenho apenas R$10.000,00 para investir no negócio.",
				"Banqueiro|Minha proposta é a de um empréstimo de R$40.000,00 com taxa de juros de 2% ao mês, aproximadamente 26,82% ao ano. O que acha?",
				"Bob Fisher|Acho excelente! Já tenho um barco em mente que custa R$25.000,00 e com este dinheiro poderei compra-lo. Também poderei pagar a taxa de R$5.000,00 pela licença de pesca. Porém notei que o salário mínimo atual é de R$1.250,00...",
				"Governador|Certo, este é o valor.",
				"Bob Fisher|Devido aos custos que terei como a compra de um barco e a licença, para viabilizar o negócio eu gostaria de contratar 5 funcionários, mas para isso o Governador precisará reduzir o salário mínimo para R$1.125,00.",
				"Governador|Bem, este tipo de medida não é nada popular... mas se você conseguir montar a empresa, ela prosperar e gerar mais empregos e melhores salários no futuro próximo, posso concordar.",
				"Bob Fisher|Ótimo, assumo tal compromisso! Vou pagar então o valor de R$10.718,75 de impostos ao governo e R$5.625,00 em salários este mês. Estou animado para começar o trabalho."
			},
			new string[] {
				// LEVEL 2
				"Bob Fisher|Peixes e alimentos são uma grande e importante riqueza! No último trimestre vendemos R$39.000,00 em peixes que alimentaram as famílias ao longo de todo o estado de Porto dos Peixes! E pagamos R$3.937,50 de impostos e R$11.250,00 de salários.",
				"Bob Fisher|Em geral, cada funcionário produziu R$2.000,00 por mês e eu produzi R$3.000,00; provavelmente porque tenho mais experiência no ramo e trabalhei algumas horas a mais.",
				"Bob Fisher|Eu amo o que faço e pretendo construir uma empresa grande e forte, e sei que para isso é necessário muito trabalho duro! Até o momento não obtive lucro algum, mas com o tempo será possível pagar o investimento inicial e a dívida que tenho com o banco, para então começar a ter lucro.",
				"Bob Fisher|Com a diminuição do salário mínimo, outras empresas, não somente a minha, contrataram mais funcionários diminuindo a taxa de desemprego. E mais do que isso, com mais força de trabalho, mais riquezas foram criadas! Essa combinação trouxe mais prosperidade à Porto dos Peixes.",
				"Bob Fisher|Sr. Governador, dados os bons resultados do último trimestre, o que acha de diminuir a quantidade de impostos que o estado cobra sobre as empresas e sua população? Eu lhe garanto que tal redução resultará em mais prosperidade, pois poderemos contratar ainda mais funcionários.",
				"Governador|Sr. Fisher, eu aprendi que reduzir a interferência do estado sobre a vida das pessoas resulta no florescimento da liberdade e prosperidade do indivíduo. Sendo assim, reduzirei esse trimestre o imposto pago pela população para 15% e 25% para empresas.",
				"Bob Fisher|Excelente! Como as coisas estão indo bem, farei mais um investimento: contratarei mais 5 funcionários.",
				"Bob Fisher|Agora, faço a seguinte proposta aos meus 10 funcionários: irei aumentar os salários para R$1.400,00 este mês e se vocês produzirem em média R$2.500,00 em peixes por mês, manterei tal valor."
			},
			new string[] {
				// LEVEL 3
				"Bob Fisher|No último trimestre vendemos R$84.000,00 em peixes e a média de produção por funcionário subiu para R$2.500,00 por mês! Eles gostaram do incentivo e trabalharam duro para atingirem o objetivo. Sendo assim, esta empresa continuará pagando R$1.400,00 de salário. Parabéns a todos!",
				"Bob Fisher|Também pagamos R$10.500,00 de imposto e R$42.000,00 de salários.",
				"Bob Fisher|Sr. Governador, como deve ter observado, a redução de impostos permitiu as empresas a contratarem mais e aos funcionários a manterem mais do fruto de seu trabalho!",
				"Governador|Sem dúvidas, Sr. Fisher. Após esta experiência, estou preparado para reduzir o imposto pago pela população para 10% e 15% para empresas. Continue o bom trabalho!",
				"Bob Fisher|Pois esta é uma excelente notícia, Sr. Governador. Sendo assim, vejo uma grande oportunidade de maiores investimentos em Porto dos Peixes! Contratarei mais 25 funcionários e mais do que dobrarei a nossa produção de peixes!",
				"Bob Fisher|Isso é possível, pois como o povo mantém mais do dinheiro de seus salários, ele irá consumir mais e nós também temos condições de produzir mais, buscando assim o equilíbrio entre demanda e oferta.",
				"Bob Fisher|Além disso, com os ganhos que estamos tendo, acredito que no próximo trimestre conseguiremos receita suficiente para pagar nossa dívida com o banco!"
			},
			new string[] {
				// LEVEL 4
				"Bob Fisher|O estado de Porto dos Peixes está cada vez mais próspero! O desemprego é o mais baixo já registrado e isso significa que mais e mais riquezas são produzidas todos os dias.",
				"Bob Fisher|Algumas pessoas acreditam que dinheiro é riqueza... o que é um tremendo engano! Dinheiro nada mais é do que uma representação da riqueza. A riqueza de verdade está nas fazendas de trigo e de grãos, no gado, nos porcos, nas galinhas e, claro, nos peixes que alimentam as famílias desse maravilhoso lugar!",
				"Bob Fisher|Aquilo que produzimos, sejam alimentos, objetos, eletrodomésticos, bens ou serviços é que são de fato valiosos! Sem tais produtos ou sem o trabalho duro que cada um de nós coloca no processo de produção, o dinheiro não passaria de papel sem valor.",
				"Bob Fisher|No último trimestre vendemos R$271.500,00 em peixes e pagamos R$22.050,00 de imposto e R$147.000,00 de salários. Finalmente, a empresa tem mais dinheiro em caixa do que deve ao banco. Portanto, irei pagar o valor de R$47.803,70 ao banco e quitarei a nossa dívida!",
				"Banqueiro|Eu sabia que estava apostando no cavalo vencedor Sr. Fisher! Foi um enorme prazer participar da fundação de uma das mais importantes companhias de Porto dos Peixes.",
				"Bob Fisher|Eu que lhe agradeço! Espero que possamos fazer mais negócios no futuro e trabalhar mais em novos projetos. Mas por ora, vou pescar mais alguns valiosos peixes!"
			},
			new string[] {
				// LEVEL 5
				"Bob Fisher|Eu sempre tive o grande sonho de construir uma empresa de pesca bem-sucedida... hoje esse sonho se torna realidade! Após muito estudo e trabalho duro, não só da minha parte, mas de cada um dos meus 35 funcionários, a companhia teve seu primeiro trimestre com lucro!",
				"Bob Fisher|Nos últimos 3 meses pagamos R$22.050,00 de impostos e R$147.000,00 de salários. Em contra partida, vendemos R$271.500,00 em peixes e, pela primeira vez, não temos dívida com ninguém. Após tanto esforço, me sinto realizado e empenhado em prosseguir!",
				"Bob Fisher|A busca pela felicidade está em conquistar seu próprio sucesso, este foi o pensamento que me trouxe até aqui e que me inspirou a não medir esforços para lutar pelos meus objetivos!",
				"Novo Governador|Muito bonitas suas palavras Sr. Fisher... pena que você parece se esquecer de pensar nos outros, como nos seus funcionários.",
				"Bob Fisher|Me desculpe, mas... quem é o senhor e por que diz isso?",
				"Novo Governador|Ahh, o senhor não ficou sabendo... eu sou o novo Governador de Porto dos Peixes. A política de redução do salário mínimo do antigo governador lhe custou caro com a população! Sendo assim, fui eleito para trazer justiça social aos trabalhadores e aos mais pobres, por quem você parece não se importar.",
				"Bob Fisher|Como pode dizer que não me importo com meus trabalhadores ou com o povo de Porto dos Peixes? A razão pela qual minha companhia existe é justamente para trazer prosperidade a eles, o que aconteceu durante o mandato do governador anterior.",
				"Novo Governador|Se você se importasse de verdade com os mais pobres, você não cobraria tão caro pelos peixes! E se si importasse com seus trabalhadores, não pagaria salários tão baixos! E se o governador anterior se importasse, ele não teria reduzido o salário mínimo.",
				"Bob Fisher|O salário que eu pago é superior ao antigo mínimo, antes da redução e tão redução permitiu a contratação de mais funcionários, diminuindo o desemprego. Não percebe os ganhos que o estado teve e o quão próspero ele é hoje?",
				"Novo Governador|Sr. Fisher, a verdade é que o que levou à diminuição do salário do povo foi a sua ganância! Sendo assim, em nome da justiça social, estou aumentando o salário mínimo para R$2.000,00 e os impostos pagos pelas empresas para 35%.",
				"Bob Fisher|Isso é um absurdo! Receio que os efeitos serão catastróficos no futuro próximo... tenho que dar um jeito de abrir os olhos desse novo governador para que ele desfaça essa medida. Mas agora, está na hora de pescar."
			},
			new string[] {
				// LEVEL 6
				"Bob Fisher|No último trimestre vendemos R$271.500,00 em peixes, pagamos R$73.500,00 de impostos e R$210.000,00 de salários. Nesses últimos meses tivemos prejuízo e nosso dinheiro em caixa diminuiu devido aos gastos! Gastamos mais com pessoal e impostos do que podemos.",
				"Bob Fisher|Devido ao imposto abusivo e o salário mínimo absurdo, tivemos prejuízo de R$200,00 por mês por funcionário. Uma vez que para os índices atuais, manter um funcionário custa R$2.700,00 por mês, embora cada funcionário produza apenas R$2.500,00 por mês.",
				"Bob Fisher|Diante deste problema, serei obrigado a demitir 5 funcionários para diminuir as perdas. E se a situação não for melhorada em breve, infelizmente, mais demissões estarão por vir.",
				"Bob Fisher|Observe que outras empresas já perceberam isso e estão tomando medidas para amenizarem suas perdas... o desemprego voltou a subir em Porto dos Peixes.",
				"Bob Fisher|Sr. Governador, por favor, tome providências e reverta a medida de aumento de impostos e salários que impôs há 3 meses!",
				"Novo Governador|De maneira alguma, Sr. Fisher! Eu estou aqui para ajudar o povo e ele exige que você pague melhores salários e diminua o preço do peixe.",
				"Bob Fisher|O senhor parece não ter entendido governador. Estou tendo prejuízos! Não posso diminuir preços, muito pelo contrário, para manter meus funcionários ou eles terão de produzir mais ou eu terei de aumentar o preço do peixe.",
				"Novo Governador|Já que, em nome de sua ganância, você se recusa a abrir mão do lucro em prol do povo, estou criando uma nova legislação para lhe proibir de aumentar os preços, além de subir o salário dos trabalhadores para R$2.500,00 e o imposto das empresas para 50%!",
				"Bob Fisher|O que o senhor precisa aprender é que quando você simplesmente aumenta o salário dos trabalhadores sem eles terem que conquistar tal aumento, eles irão gastar mais e produzir o mesmo tanto.",
				"Bob Fisher|Gastar/consumir é o processo de destruição de riquezas! Sem que se produza mais riquezas para repor a oferta, os preços sobem para frear o consumo elevado.",
				"Bob Fisher|Se você me impedir de aumentar os preços e me obrigar a pagar mais impostos, não irá demorar muito até que eu tenha que fechar minha empresa!",
				"Novo Governador|Você só pensa assim porque é um capitalista rico que não se importa com o fato de que existem pessoas que não tem R$10,00 para comprar o peixe que você vende às custas do trabalho dos outros.",
				"Novo Governador|Só está preocupado com lucro e mais lucro... Já eu irei utilizar o dinheiro dos seus impostos para dar para o povo comprar alimentos.",
				"Bob Fisher|Não Sr. Governador, não estou preocupado com meu lucro. Estou preocupado com os efeitos que o intervencionismo estatal que você está provocando na economia de Porto dos Peixes irá causar na população que o senhor diz defender."
			},
			new string[] {
				// LEVEL 7
				"Bob Fisher|Estou preocupado com o futuro deste estado. As políticas dos últimos 6 meses aumentaram o desemprego quase que para o valor de quando cheguei a Porto dos Peixes.",
				"Bob Fisher|Com isso, houve uma grande redução na prosperidade já que empresas foram fechadas e fazer negócios e produzir riqueza tem sido cada vez mais difícil.",
				"Bob Fisher|Algumas pessoas acreditam que taxar os ricos ou as empresas é uma coisa boa. Uma forma de justiça social. Uma revanche dos 99% contra o 1% da população que faz parte do clube dos ricos. Mas a verdade é que elas se esquecem de alguns pontos importantes.",
				"Bob Fisher|O primeiro é que quando você taxa os mais ricos, eles vão embora para um lugar onde há livre mercado e espaço para a produção de riquezas.",
				"Bob Fisher|O segundo é que eles levam consigo suas empresas e seu dinheiro, retirando uma grande quantidade de empregos e investimentos que tinham no lugar onde viviam.",
				"Bob Fisher|O terceiro é que o clube dos ricos está sempre aberto para novos membros, se você tiver uma ideia inovadora ou a visão de um novo negócio pode alcançar tal grupo.",
				"Bob Fisher|E por fim, algumas pessoas parecem não perceber que não é necessário fazer parte dos 1% para se ter uma ótima vida!",
				"Bob Fisher|Algumas pessoas que acusam os outros de serem gananciosos parecem nem perceberem o quanto sua própria ganância cresceu!",
				"Bob Fisher|No último trimestre vendemos R$234.000,00 em peixes, pagamos R$112.500,00 de impostos e R$225.000,00 de salários. O caixa da empresa se reduziu a praticamente um terço do que era há 3 meses.",
				"Bob Fisher|Hoje o custo de um funcionário é de R$3.750,00 por mês, ou seja, R$1.250,00 a mais do que ele produz.",
				"Bob Fisher|Está impossível continuar, portanto Sr. Governador, estou demitindo mais 20 funcionários e condicionando a continuação da empresa à revogação das últimas políticas de impostos e salários.",
				"Novo Governador|Sr. Fisher, não irei ceder a sua pressão. Se sua empresa não é capaz de se manter, então talvez o estado devesse assumir o controle dela.",
				"Bob Fisher|Não é a minha empresa que não é capaz de se manter... empresa alguma consegue se manter nesse estado desde que você assumiu o governo!",
				"Bob Fisher|Em termos de economia, este estado deixou de ser um dos mais liberais e prósperos para ser um dos mais burocráticos, ineficientes e caros.",
				"Novo Governador|Pois se a economia vai mal, basta darmos mais dinheiro para a população, pois isso aquece a economia e a faz girar! Logo, passarei o salário mínimo para R$3.500,00 e o imposto de empresas para 77%.",
				"Bob Fisher|O que você chama de \"aquecimento da economia\" não passa de mais destruição de riqueza. E empresa alguma nesse estado tem condições de produzir mais riquezas, pois você está destruindo tudo."
			},
			new string[] {
				// LEVEL 8
				"Bob Fisher|No último trimestre vendemos R$84.000,00 em peixes, pagamos R$80.850,00 de impostos e R$105.000,00 de salários. Pela primeira vez, o prejuízo acumulado foi além do dinheiro que tínhamos em caixa e estou endividado. Já não acredito que haverá volta dessa situação.",
				"Bob Fisher|Estou demitindo mais 5 funcionários e agora estou à procura de interessados em comprar minha empresa. Infelizmente, estou falido. Não somente eu, mas sim quase todos neste estado. O desemprego é de 50% e só cresce.",
				"Bob Fisher|Cada vez menos riquezas são produzidas, mas ainda assim as pessoas continuam a consumir sem pensar no amanhã e querendo cada vez mais!",
				"Bob Fisher|Um ponto muito importante na economia é que ela se trata do estudo da escassez. Vivemos em um mundo onde os recursos (matérias-primas e mão-de-obra) são escassos.",
				"Bob Fisher|É necessário saber administra-los a fim de permitir que as pessoas sempre tenham o que querem e desejam.",
				"Bob Fisher|Embora a produção em Porto dos Peixes esteja cada vez menor, o consumo continua alto, as pessoas querem cada vez mais coisas e não percebem que essas coisas são escassas e que se elas não trabalharem duro o suficiente para produzir o tanto que consomem, elas irão faltar!"
			},
			new string[] {
				// END
				"Bob Fisher|Vou me preparar para deixar o estado e ir em busca de melhores oportunidades em algum outro lugar. Mas ainda tenho de resolver o problema de minha dívida.",
				"Bob Fisher|Nos últimos 3 meses vendemos R$46.500,00 em peixes, pagamos R$40.425,00 de impostos e R$52.500,00 de salários. A dívida atual é de R$47.709,95.",
				"Bob Fisher|Acho que encontrei alguém disposto a comprar a minha empresa e meu barco por um valor próximo a R$50.000,00. Isso seria suficiente para sanar minha dívida e pagar uma passagem de saída daqui.",
				"Bob Fisher|Mas antes tenho de demitir os meus últimos 5 funcionários.",
				"Novo Governador|Não tão rápido Sr. Fisher... chega dessas demissões de funcionários! Se os empresários preferem salvar os próprios bolsos, está na hora de o estado nacionalizar suas empresas e recontratar os funcionários!",
				"Bob Fisher|O senhor não pode estatizar minha companhia! Eu a construí com muito esforço e dedicação, já não basta você ter afundado ela com suas políticas intervencionistas, não irá rouba-la de mim.",
				"Novo Governador|Não estou roubando, está tudo dentro da lei. Sua empresa agora pertence ao estado e tudo que você irá receber são R$25.000,00 como indenização pelo seu barco.",
				"Bob Fisher|Mas minha empresa ainda vale mais do que isso! E isso ainda me deixa com uma dívida de R$22.709,95.",
				"Novo Governador|Que a propósito é melhor você pagar em breve, se não quiser ser preso. Agora saia das instalações da minha empresa!",
				"Bob Fisher|Eu perdi tudo... deveria ter ido embora junto com os outros empresários logo após esse governador ter chegado ao poder.",
				"Bob Fisher|Ele acredita que será capaz de usar o estado para produzir riqueza, mas a verdade é que com tantas pessoas ficando desempregadas e empresas fechando, o recolhimento de impostos com certeza diminuiu muito! E em breve, o próprio estado irá falir."
			}
		};

		private static readonly Vector2 displaySize = new Vector2(150, 91.5f);

		private static readonly Bitmap[] PROSP_TEXTURES = new Bitmap[8] {
			new Bitmap("./Textures/PROSPERITY_15.png"),
			new Bitmap("./Textures/PROSPERITY_25.png"),
			new Bitmap("./Textures/PROSPERITY_35.png"),
			new Bitmap("./Textures/PROSPERITY_50.png"),
			new Bitmap("./Textures/PROSPERITY_60.png"),
			new Bitmap("./Textures/PROSPERITY_75.png"),
			new Bitmap("./Textures/PROSPERITY_85.png"),
			new Bitmap("./Textures/PROSPERITY_90.png")
		};

		private static readonly Bitmap[] UNEMPL_TEXTURES = new Bitmap[8] {
			new Bitmap("./Textures/UNEMPLOYMENT_3.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_5.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_13.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_20.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_35.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_40.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_50.png"),
			new Bitmap("./Textures/UNEMPLOYMENT_60.png")
		};

		private static readonly Bitmap[] TAXES_BUSY_TEXTURES = new Bitmap[5] {
			new Bitmap("./Textures/TAXES_COMPANIES_15.png"),
			new Bitmap("./Textures/TAXES_COMPANIES_25.png"),
			new Bitmap("./Textures/TAXES_COMPANIES_35.png"),
			new Bitmap("./Textures/TAXES_COMPANIES_50.png"),
			new Bitmap("./Textures/TAXES_COMPANIES_77.png")
		};

		private static readonly Bitmap[] TAXES_PEOPLE_TEXTURES = new Bitmap[3] {
			new Bitmap("./Textures/TAXES_PEOPLE_10.png"),
			new Bitmap("./Textures/TAXES_PEOPLE_15.png"),
			new Bitmap("./Textures/TAXES_PEOPLE_20.png")
		};

		private static readonly Bitmap[] CASH_TEXTURES = new Bitmap[11] {
			new Bitmap("./Textures/CASH_0.png"),
			new Bitmap("./Textures/CASH_2618,75.png"),
			new Bitmap("./Textures/CASH_3656,25.png"),
			new Bitmap("./Textures/CASH_9968,75.png"),
			new Bitmap("./Textures/CASH_10000.png"),
			new Bitmap("./Textures/CASH_38615,05.png"),
			new Bitmap("./Textures/CASH_50000.png"),
			new Bitmap("./Textures/CASH_57265,05.png"),
			new Bitmap("./Textures/CASH_91565,05.png"),
			new Bitmap("./Textures/CASH_105068,75.png"),
			new Bitmap("./Textures/CASH_121565,05.png")
		};

		private static readonly Bitmap[] DEBT_TEXTURES = new Bitmap[8] {
			new Bitmap("./Textures/DEBT_0.png"),
			new Bitmap("./Textures/DEBT_22709,95.png"),
			new Bitmap("./Textures/DEBT_32259,95.png"),
			new Bitmap("./Textures/DEBT_40000.png"),
			new Bitmap("./Textures/DEBT_42448,32.png"),
			new Bitmap("./Textures/DEBT_45046,49.png"),
			new Bitmap("./Textures/DEBT_47709,95.png"),
			new Bitmap("./Textures/DEBT_47803,70.png")
		};

		private static readonly Bitmap[] SALARY_TEXTURES = new Bitmap[6] {
			new Bitmap("./Textures/SALARY_1125.png"),
			new Bitmap("./Textures/SALARY_1250.png"),
			new Bitmap("./Textures/SALARY_1400.png"),
			new Bitmap("./Textures/SALARY_2000.png"),
			new Bitmap("./Textures/SALARY_2500.png"),
			new Bitmap("./Textures/SALARY_3500.png")
		};

		private static readonly Bitmap[] WORKERS_TEXTURES = new Bitmap[5] {
			new Bitmap("./Textures/WORKERS_0.png"),
			new Bitmap("./Textures/WORKERS_5.png"),
			new Bitmap("./Textures/WORKERS_10.png"),
			new Bitmap("./Textures/WORKERS_30.png"),
			new Bitmap("./Textures/WORKERS_35.png")
		};

		private static readonly Bitmap[] CHARACTERS_TEXTURES = new Bitmap[4] {
			new Bitmap("./Textures/MAN_1.png"),
			new Bitmap("./Textures/MAN_2.png"),
			new Bitmap("./Textures/POLITICIAN_1.png"),
			new Bitmap("./Textures/POLITICIAN_2.png")
		};

		private static uint[] PROSP_TEXTURES_IDS = new uint[PROSP_TEXTURES.Length];
		private static uint[] UNEMPL_TEXTURES_IDS = new uint[UNEMPL_TEXTURES.Length];
		private static uint[] TAXES_BUSY_TEXTURES_IDS = new uint[TAXES_BUSY_TEXTURES.Length];
		private static uint[] TAXES_PEOPLE_TEXTURES_IDS = new uint[TAXES_PEOPLE_TEXTURES.Length];

		private static uint[] CASH_TEXTURES_IDS = new uint[CASH_TEXTURES.Length];
		private static uint[] DEBT_TEXTURES_IDS = new uint[DEBT_TEXTURES.Length];
		private static uint[] SALARY_TEXTURES_IDS = new uint[SALARY_TEXTURES.Length];
		private static uint[] WORKERS_TEXTURES_IDS = new uint[WORKERS_TEXTURES.Length];
		private static uint[] CHARACTERS_TEXTURES_IDS = new uint[CHARACTERS_TEXTURES.Length];

		public static int LEVEL = 0;
		private static int currentSpeech = 0;
		private static GameObject[] indexes;

		public static void NextLevel () {
			if (++LEVEL > 9)
				LEVEL = 0;
		}

		public static void NextSpeech () {
			if (++currentSpeech >= speeches[LEVEL].Length) {
				if (LEVEL < 8) {
					// Set the player's goal and load the game level
					Game.Goal = levelGoal[LEVEL];
					SceneManager.LoadScene("GAME");
				} else if (LEVEL == 8) {
					// Show last quote
					NextLevel();
					SceneManager.ReleadLevel();
				}
			} else {
				if (LEVEL == 0 && currentSpeech == 7) {
					indexes[6].Animator.AddTexture("DISPLAY_6", CASH_TEXTURES_IDS[6]);
					indexes[6].Animator.CurrentTexture = "DISPLAY_6";

					indexes[7].Animator.AddTexture("DISPLAY_3", DEBT_TEXTURES_IDS[3]);
					indexes[7].Animator.CurrentTexture = "DISPLAY_3";
				} else if (LEVEL == 0 && currentSpeech == 11) {
					indexes[4].Animator.AddTexture("DISPLAY_1", WORKERS_TEXTURES_IDS[1]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_1";

					indexes[5].Animator.AddTexture("DISPLAY_0", SALARY_TEXTURES_IDS[0]);
					indexes[5].Animator.CurrentTexture = "DISPLAY_0";

					indexes[6].Animator.AddTexture("DISPLAY_2", CASH_TEXTURES_IDS[2]);
					indexes[6].Animator.CurrentTexture = "DISPLAY_2";
				} else if (LEVEL == 1 && currentSpeech == 5) {
					indexes[3].Animator.AddTexture("DISPLAY_1", TAXES_BUSY_TEXTURES_IDS[1]);
					indexes[3].Animator.CurrentTexture = "DISPLAY_1";

					indexes[2].Animator.AddTexture("DISPLAY_1", TAXES_PEOPLE_TEXTURES_IDS[1]);
					indexes[2].Animator.CurrentTexture = "DISPLAY_1";
				} else if (LEVEL == 1 && currentSpeech == 6) {
					indexes[4].Animator.AddTexture("DISPLAY_2", WORKERS_TEXTURES_IDS[2]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_2";
				} else if (LEVEL == 1 && currentSpeech == 7) {
					indexes[5].Animator.AddTexture("DISPLAY_2", SALARY_TEXTURES_IDS[2]);
					indexes[5].Animator.CurrentTexture = "DISPLAY_2";
				} else if (LEVEL == 2 && currentSpeech == 3) {
					indexes[3].Animator.AddTexture("DISPLAY_0", TAXES_BUSY_TEXTURES_IDS[0]);
					indexes[3].Animator.CurrentTexture = "DISPLAY_0";

					indexes[2].Animator.AddTexture("NEW_DISPLAY", TAXES_PEOPLE_TEXTURES_IDS[0]);
					indexes[2].Animator.CurrentTexture = "NEW_DISPLAY";
				} else if (LEVEL == 2 && currentSpeech == 4) {
					indexes[4].Animator.AddTexture("DISPLAY_4", WORKERS_TEXTURES_IDS[4]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_4";
				} else if (LEVEL == 3 && currentSpeech == 3) {
					indexes[7].Animator.AddTexture("DISPLAY_0", DEBT_TEXTURES_IDS[0]);
					indexes[7].Animator.CurrentTexture = "DISPLAY_0";

					indexes[6].Animator.AddTexture("DISPLAY_7", CASH_TEXTURES_IDS[7]);
					indexes[6].Animator.CurrentTexture = "DISPLAY_7";
				} else if (LEVEL == 4 && currentSpeech == 9) {
					indexes[5].Animator.AddTexture("DISPLAY_3", SALARY_TEXTURES_IDS[3]);
					indexes[5].Animator.CurrentTexture = "DISPLAY_3";

					indexes[3].Animator.AddTexture("DISPLAY_2", TAXES_BUSY_TEXTURES_IDS[2]);
					indexes[3].Animator.CurrentTexture = "DISPLAY_2";
				} else if (LEVEL == 5 && currentSpeech == 2) {
					indexes[4].Animator.AddTexture("DISPLAY_3", WORKERS_TEXTURES_IDS[3]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_3";
				} else if (LEVEL == 5 && currentSpeech == 7) {
					indexes[5].Animator.AddTexture("DISPLAY_4", SALARY_TEXTURES_IDS[4]);
					indexes[5].Animator.CurrentTexture = "DISPLAY_4";

					indexes[3].Animator.AddTexture("DISPLAY_3", TAXES_BUSY_TEXTURES_IDS[3]);
					indexes[3].Animator.CurrentTexture = "DISPLAY_3";
				} else if (LEVEL == 6 && currentSpeech == 10) {
					indexes[4].Animator.AddTexture("DISPLAY_2", WORKERS_TEXTURES_IDS[2]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_2";
				} else if (LEVEL == 6 && currentSpeech == 14) {
					indexes[5].Animator.AddTexture("DISPLAY_5", SALARY_TEXTURES_IDS[5]);
					indexes[5].Animator.CurrentTexture = "DISPLAY_5";

					indexes[3].Animator.AddTexture("DISPLAY_4", TAXES_BUSY_TEXTURES_IDS[4]);
					indexes[3].Animator.CurrentTexture = "DISPLAY_4";
				} else if (LEVEL == 7 && currentSpeech == 1) {
					indexes[4].Animator.AddTexture("DISPLAY_1", WORKERS_TEXTURES_IDS[1]);
					indexes[4].Animator.CurrentTexture = "DISPLAY_1";
				} else if (LEVEL == 8 && currentSpeech == 6) {
					indexes[7].Animator.AddTexture("DISPLAY_1", DEBT_TEXTURES_IDS[1]);
					indexes[7].Animator.CurrentTexture = "DISPLAY_1";
				}
			}
		}

		public StoryManager (OpenGL gl) {
			this.gl = gl;

			if (!texturesRegistered) {
				for (int i = 0; i < PROSP_TEXTURES_IDS.Length; i++)
					PROSP_TEXTURES_IDS[i] = Animator.RegisterTexture(PROSP_TEXTURES[i]);

				for (int i = 0; i < UNEMPL_TEXTURES_IDS.Length; i++)
					UNEMPL_TEXTURES_IDS[i] = Animator.RegisterTexture(UNEMPL_TEXTURES[i]);

				for (int i = 0; i < TAXES_BUSY_TEXTURES_IDS.Length; i++)
					TAXES_BUSY_TEXTURES_IDS[i] = Animator.RegisterTexture(TAXES_BUSY_TEXTURES[i]);

				for (int i = 0; i < TAXES_PEOPLE_TEXTURES_IDS.Length; i++)
					TAXES_PEOPLE_TEXTURES_IDS[i] = Animator.RegisterTexture(TAXES_PEOPLE_TEXTURES[i]);

				for (int i = 0; i < CASH_TEXTURES_IDS.Length; i++)
					CASH_TEXTURES_IDS[i] = Animator.RegisterTexture(CASH_TEXTURES[i]);

				for (int i = 0; i < DEBT_TEXTURES_IDS.Length; i++)
					DEBT_TEXTURES_IDS[i] = Animator.RegisterTexture(DEBT_TEXTURES[i]);

				for (int i = 0; i < SALARY_TEXTURES_IDS.Length; i++)
					SALARY_TEXTURES_IDS[i] = Animator.RegisterTexture(SALARY_TEXTURES[i]);

				for (int i = 0; i < WORKERS_TEXTURES_IDS.Length; i++)
					WORKERS_TEXTURES_IDS[i] = Animator.RegisterTexture(WORKERS_TEXTURES[i]);

				for (int i = 0; i < CHARACTERS_TEXTURES_IDS.Length; i++)
					CHARACTERS_TEXTURES_IDS[i] = Animator.RegisterTexture(CHARACTERS_TEXTURES[i]);

				texturesRegistered = true;
			}
		}

		private OpenGL gl;
		private Label label;
		private Label speech;
		private DateTime start;
		private GameObject character;
		private GameObject background;
		private GameObject backgroundBottom;
		private GameObject indexesBackground;

		public void InitScene () {
			background = new GameObject(new Vector2(SceneManager.ScreenSize.X * 0.825f, 2000 / 3000f * SceneManager.ScreenSize.X * 0.825f, SceneManager.ScreenSize.X, SceneManager.ScreenSize.Y));
			background.Animator.AddTexture("MAP", new Bitmap("./Textures/MAP.png"));
			background.Animator.CurrentTexture = "MAP";
			background.IsHidden = true;
			SceneManager.AddObject(background);

			label = new Label(quotes[LEVEL], 32, Color.White, SceneManager.ScreenSize.WithRef(SceneManager.ScreenSize), Label.Alignment.CENTER, 25);
			SceneManager.AddObject(label);

			indexesBackground = new GameObject(new Vector2((displaySize.X + SPACE) * 2 + SPACE, displaySize.Y * 4 + SPACE * 5), "", new Vector2(SceneManager.ScreenSize.X - (displaySize.X + SPACE) * 2 - SPACE, 0, SceneManager.ScreenSize.X, 0, (displaySize.X + SPACE) * 2 + SPACE));
			indexesBackground.Animator.Color = Color.FromArgb(113, 10, 10, 10);
			indexesBackground.Animator.CurrentTexture = Animator.SOLID_TEXTURE;
			indexesBackground.IsHidden = true;
			SceneManager.AddObject(indexesBackground);

			indexes = new GameObject[8];
			for (int i = 0; i < 8; i++) {
				indexes[i] = new GameObject(displaySize, "", new Vector2(SceneManager.ScreenSize.X - (displaySize.X + SPACE) * (i % 2 + 1), (displaySize.Y + SPACE) * (i / 2) + SPACE, SceneManager.ScreenSize.X, 0, (displaySize.X + SPACE) * (i % 2 + 1)));
				indexes[i].Animator.AddTexture("DISPLAY", GetTexture(i));
				indexes[i].Animator.CurrentTexture = "DISPLAY";
				indexes[i].IsHidden = true;
				SceneManager.AddObject(indexes[i]);
			}

			backgroundBottom = new GameObject(new Vector2(SceneManager.ScreenSize.X, 200, SceneManager.ScreenSize.X, 0), "", new Vector2(0, SceneManager.ScreenSize.Y - 200, 0, SceneManager.ScreenSize.Y, 0, 200));
			backgroundBottom.Animator.Color = Color.FromArgb(113, 10, 10, 10);
			backgroundBottom.Animator.CurrentTexture = Animator.SOLID_TEXTURE;
			backgroundBottom.IsHidden = true;
			SceneManager.AddObject(backgroundBottom);

			currentSpeech = 0;
			character = new GameObject(new Vector2(150, 150), "", new Vector2(25, SceneManager.ScreenSize.Y - 175, 0, SceneManager.ScreenSize.Y, 0, 175));
			character.IsHidden = true;
			for (int i = 0; i < CHARACTERS_TEXTURES_IDS.Length; i++)
				character.Animator.AddTexture("CHARACTER_" + i, CHARACTERS_TEXTURES_IDS[i]);

			SceneManager.AddObject(character);

			speech = new Label("", 22, Color.White, new Vector2(SceneManager.ScreenSize.X - 200, 200, SceneManager.ScreenSize.X, 0, 200));
			speech.Transform.Position = new Vector2(200, SceneManager.ScreenSize.Y - 175, 0, SceneManager.ScreenSize.Y, 0, 175);
			speech.IsHidden = true;
			SceneManager.AddObject(speech);

			start = SceneManager.Now;

			// Play games' song
			long position;
			try {
				position = OutputDevice.GetPosition();
			} catch {
				position = 0;
			}

			if (position == 0) {
				keepPlaying = true;
				OutputDevice.PlaybackStopped += SongStopped;
				OutputDevice.Init(audioFile);
				OutputDevice.Play();
			}
		}

		public void DisposeScene () {
			currentSpeech = 0;
			keepPlaying = false;
			OutputDevice.Stop();
			OutputDevice.PlaybackStopped -= SongStopped;
			audioFile.Seek(0, System.IO.SeekOrigin.Begin);
		}

		private void SongStopped (object sender, StoppedEventArgs e) {
			if (keepPlaying) {
				audioFile.Seek(0, System.IO.SeekOrigin.Begin);
				OutputDevice.Play();
			}
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			// Background color
			gl.ClearColor(51 / 255f, 102 / 255f, 153 / 255f, 0f);    // Ocean

			if (indexes[0].IsHidden && SceneManager.Now.Subtract(start).TotalMilliseconds > idleDurations[LEVEL]) {
				if (LEVEL == 9) {
					NextLevel();
					MenuHUD.ShowAbout = true;
					SceneManager.LoadScene("MENU");
				}
				label.IsHidden = true;
				indexesBackground.IsHidden = background.IsHidden = backgroundBottom.IsHidden = character.IsHidden = speech.IsHidden = false;
				for (int i = 0; i < indexes.Length; i++)
					indexes[i].IsHidden = false;
			} else if (!indexes[0].IsHidden) {
				character.Animator.CurrentTexture = GetCurrentCharacter();
				string txt = speeches[LEVEL].Length > currentSpeech ? speeches[LEVEL][currentSpeech] : "";
				speech.Text = txt.Substring(txt.IndexOf("|") + 1);
			}
		}

		public string GetCurrentCharacter () {
			string txt = speeches[LEVEL].Length > currentSpeech ? speeches[LEVEL][currentSpeech] : "";
			string character = txt.Substring(0, txt.IndexOf("|"));
			switch (character) {
				case "Bob Fisher":
					return "CHARACTER_0";
				case "Banqueiro":
					return "CHARACTER_1";
				case "Governador":
					return "CHARACTER_2";
				default:
					return "CHARACTER_3";
			}
		}

		private uint GetTexture (int index) {
			switch (index) {
				case 0:
					switch (LEVEL) {
						case 1:
							return PROSP_TEXTURES_IDS[2];
						case 2:
							return PROSP_TEXTURES_IDS[4];
						case 3:
							return PROSP_TEXTURES_IDS[6];
						case 4:
							return PROSP_TEXTURES_IDS[7];
						case 5:
							return PROSP_TEXTURES_IDS[5];
						case 6:
							return PROSP_TEXTURES_IDS[3];
						case 7:
							return PROSP_TEXTURES_IDS[1];
						default:
							return PROSP_TEXTURES_IDS[0];
					}
				case 1:
					switch (LEVEL) {
						case 0:
							return UNEMPL_TEXTURES_IDS[5];
						case 1:
							return UNEMPL_TEXTURES_IDS[3];
						case 2:
							return UNEMPL_TEXTURES_IDS[1];
						case 3:
						case 4:
							return UNEMPL_TEXTURES_IDS[0];
						case 5:
							return UNEMPL_TEXTURES_IDS[2];
						case 6:
							return UNEMPL_TEXTURES_IDS[4];
						case 7:
							return UNEMPL_TEXTURES_IDS[6];
						default:
							return UNEMPL_TEXTURES_IDS[7];
					}
				case 2:
					switch (LEVEL) {
						case 0:
						case 1:
							return TAXES_PEOPLE_TEXTURES_IDS[2];
						case 2:
							return TAXES_PEOPLE_TEXTURES_IDS[1];
						default:
							return TAXES_PEOPLE_TEXTURES_IDS[0];
					}
				case 3:
					switch (LEVEL) {
						case 0:
						case 1:
						case 5:
							return TAXES_BUSY_TEXTURES_IDS[2];
						case 2:
							return TAXES_BUSY_TEXTURES_IDS[1];
						case 3:
						case 4:
							return TAXES_BUSY_TEXTURES_IDS[0];
						case 6:
							return TAXES_BUSY_TEXTURES_IDS[3];
						default:
							return TAXES_BUSY_TEXTURES_IDS[4];
					}
				case 4:
					switch (LEVEL) {
						case 1:
							return WORKERS_TEXTURES_IDS[1];
						case 2:
						case 7:
							return WORKERS_TEXTURES_IDS[2];
						case 3:
						case 4:
						case 5:
							return WORKERS_TEXTURES_IDS[4];
						case 6:
							return WORKERS_TEXTURES_IDS[3];
						case 8:
							return WORKERS_TEXTURES_IDS[1];
						default:
							return WORKERS_TEXTURES_IDS[0];
					}
				case 5:
					switch (LEVEL) {
						case 0:
							return SALARY_TEXTURES_IDS[1];
						case 1:
							return SALARY_TEXTURES_IDS[0];
						case 2:
						case 3:
						case 4:
							return SALARY_TEXTURES_IDS[2];
						case 5:
							return SALARY_TEXTURES_IDS[3];
						case 6:
							return SALARY_TEXTURES_IDS[4];
						default:
							return SALARY_TEXTURES_IDS[5];
					}
				case 6:
					switch (LEVEL) {
						case 0:
							return CASH_TEXTURES_IDS[4];
						case 1:
							return CASH_TEXTURES_IDS[3];
						case 2:
							return CASH_TEXTURES_IDS[1];
						case 3:
							return CASH_TEXTURES_IDS[9];
						case 4:
							return CASH_TEXTURES_IDS[10];
						case 5:
							return CASH_TEXTURES_IDS[8];
						case 6:
							return CASH_TEXTURES_IDS[5];
						default:
							return CASH_TEXTURES_IDS[0];
					}
				case 7:
					switch (LEVEL) {
						case 1:
							return DEBT_TEXTURES_IDS[4];
						case 2:
							return DEBT_TEXTURES_IDS[5];
						case 3:
							return DEBT_TEXTURES_IDS[7];
						case 7:
							return DEBT_TEXTURES_IDS[2];
						case 8:
							return DEBT_TEXTURES_IDS[6];
						default:
							return DEBT_TEXTURES_IDS[0];
					}
				default:
					return 0;
			}
		}
	}
}
