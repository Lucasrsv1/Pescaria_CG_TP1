using Pescaria_CG_TP1.Engine;
using SharpGL;
using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Scenes {
	public class StoryManager : IScene {
		private static bool texturesRegistered = false;
		private static readonly int SPACE = 10;
		private static readonly int[] idleDurations = new int[] { 12000, 8000, 8000, 13000, 15000, 6000, 15000, 8000, 12000 };
		private static readonly string[] quotes = new string[] {
			"\"Nós aprendemos com nossos pais que são os indivíduos que determinam seu próprio futuro, isto é, principalmente sua ambição e trabalho duro que determina seu destino na vida.\"\n- Ronald Reagan",
			"\"O homem não é livre a não ser que o Estado seja limitado.\nÀ medida que o governo expande, a liberdade contrai.\"\n- Ronald Reagan",
			"\"O governo deveria fazer apenas aquelas coisas que um indivíduo não pode fazer por conta própria.\"\n- Abraham Lincoln",
			"\"O capitalismo foi chamado de sistema da ganância. No entanto, é o sistema que elevou o padrão de vida de seus cidadãos mais pobres a um nível que nenhum sistema coletivista ou grupo tribal poderia jamais conceber.\"\n- Ayn Rand",
			"\"A felicidade não está relacionada ao materialismo ou a redistribuição de riqueza pelo governo. Ela está relacionada com o indivíduo definindo e buscando seus objetivos na vida. A promessa do livre mercado é permitir que o indivíduo conquiste seu próprio sucesso, buscando sua felicidade.\"\n- Arthur Brooks",
			"\"Um homem honesto é aquele que sabe que não pode consumir mais do que produziu.\"\n- Ayn Rand",
			"\"Desigualdade não é pobreza: combater a desigualdade não acaba com a pobreza e diminuir a pobreza não implica acabar com a desigualdade. É imprescindível separar esses dois conceitos para não sermos enganados pelos defensoresdo igualitarismo, os que querem apenas redistribuir a pobreza.\"\n- Mises Brasil",
			"\"O Capitalismo ensina as pessoas a trabalharem mais.\nJá o socialismo ensina as pessoas a pedirem mais.\"\n- Dennis Prager",
			"\"O inerente vício do capitalismo é o inigualitário compartilhamento de bençãos, já a inerente virtude do socialismo é o igualitário compartilhamento de misérias.\"\n- Winston Churchill"
		};

		private static readonly string[][] speeches = new string[][] {
			new string[] {
				"Governador|Olá, eu sou o governador do Porto dos Peixes. Seja bem-vindo ao nosso maravilhoso estado!",
				"Bob Fisher|Muito obrigado! Eu sou Bob Fisher e vim para cá com grandes sonhos e planos! Ouvi dizer que vocês têm águas com uma vasta variedade de peixes.",
				"Governador|Sem dúvidas! Nosso mar tem muitos peixes, porém precisamos de alguém com interesse em em explorar nossas águas.",
				"Bob Fisher|Pois então estou no lugar certo! Trabalhei por muitos anos com pesca e pretendo montar um negócio para explorar a riqueza deste mar e trazer empregos e prosperidade ao estado!",
				"Banqueiro|É muito bom ouvir isso! Prazer, eu sou o dono do Banco do Porto dos Peixes, e gostaria de lhe propor uma ajuda nessa empreitada!",
				"Bob Fisher|Pois tal ajuda será muito bem-vinda, Sr. Banqueiro. Embora tenha experiência com pesca, tenho apenas R$10.000,00 para investir no negócio.",
				"Banqueiro|Minha proposta é a de um empréstimo de R$40.000,00 com taxa de juros de 2% ao mês, aproximadamente 26,82% ao ano. O que acha?",
				"Bob Fisher|Acho excelente! Já tenho um barco em mente que custa R$25.000,00 e com este dinheiro poderei compra-lo. Também poderei pagar a taxa de R$5.000,00 pela licença de pesca. Porém notei que o salário mínimo atual é de R$1.250,00...",
				"Governador|Certo, este é o valor.",
				"Bob Fisher|Devido aos custos que terei como a compra de um barco e a licença, para viabilizar o negócio eu gostaria de contratar 5 funcionários, mas para isso o Governador precisará reduzir o salário mínimo para R$1125,00.",
				"Governador|Bem, este tipo de medida não é nada popular... mas se você conseguir montar a empresa, ela prosperar e gerar mais empregos e melhores salários no futuro próximo, posso concordar.",
				"Bob Fisher|Ótimo! Assumo tal compromisso, e estou animado para começar o trabalho."
			},
			new string[] {
				"Bob Fisher|Peixes e alimentos são uma grande e importante riqueza! No último trimestre vendemos R$39.000,00 em peixes que alimentaram as famílias ao longo de todo o estado de Porto dos Peixes! Em geral, cada funcionário produziu R$2.000,00 por mês e eu produzi R$3.000,00; provavelmente porque tenho mais experiência no ramo e trabalhei algumas horas a mais.",
				"Bob Fisher|Eu amo o que faço e pretendo construir uma empresa grande e forte, e sei que para isso é necessário muito trabalho duro! Até o momento não obtive lucro algum, mas com o tempo será possível pagar o investimento inicial e a dívida que tenho com o banco, para então começar a ter lucro.",
				"Bob Fisher|Com a diminuição do salário mínimo, outras empresas, não somente a minha, contrataram mais funcionários diminuindo a taxa de desemprego. E mais do que isso, com mais força de trabalho, mais riquezas foram criadas! Essa combinação trouxe mais prosperidade à Porto dos Peixes.",
				"Bob Fisher|Sr. Governador, dados os bons resultados do último trimestre, o que acha de diminuir a quantidade de impostos que o estado cobra sobre as empresas e sua população? Eu lhe garanto que tal redução resultará em mais prosperidade, pois poderemos contratar ainda mais funcionários.",
				"Governador|Sr. Fisher, eu aprendi que reduzir a interferência do estado sobre a vida das pessoas resulta no florescimento da liberdade e prosperidade do indivíduo. Sendo assim, reduzirei esse trimestre o imposto pago pela população para 15% e 25% para empresas.",
				"Bob Fisher|Excelente! Como as coisas estão indo bem, farei mais um investimento: contratarei mais 5 funcionários. Agora, faço a seguinte proposta aos meus 10 funcionários: irei aumentar os salários para R$1.400,00 este mês e se vocês produzirem em média R$2.500,00 em peixes por mês, manterei tal valor."
			}
		};

		private static readonly Vector2 gaugeSize = new Vector2(150, 150);
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
			if (++LEVEL > 8)
				LEVEL = 0;
		}

		public static void NextSpeech () {
			if (++currentSpeech >= speeches[LEVEL].Length) {
				switch (LEVEL) {
					case 0:
						// Set the player's goal and load the game level
						Game.Goal = 2000;
						SceneManager.LoadScene("GAME");
						break;
				}
			} else {
				if (LEVEL == 0 && currentSpeech == 7) {
					indexes[6].Animator.AddTexture("NEW_GAUGE", CASH_TEXTURES_IDS[6]);
					indexes[6].Animator.CurrentTexture = "NEW_GAUGE";
				} else if (LEVEL == 0 && currentSpeech == 11) {
					indexes[4].Animator.AddTexture("NEW_GAUGE", WORKERS_TEXTURES_IDS[1]);
					indexes[4].Animator.CurrentTexture = "NEW_GAUGE";
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

		public void DisposeScene () {
			currentSpeech = 0;
		}

		public void InitScene () {
			background = new GameObject(new Vector2(SceneManager.ScreenSize.X * 0.825f, 2000 / 3000f * SceneManager.ScreenSize.X * 0.825f, SceneManager.ScreenSize.X, SceneManager.ScreenSize.Y));
			background.Animator.AddTexture("MAP", new Bitmap("./Textures/MAP.png"));
			background.Animator.CurrentTexture = "MAP";
			background.IsHidden = true;
			SceneManager.AddObject(background);

			label = new Label(quotes[LEVEL], 32, Color.White, SceneManager.ScreenSize.WithRef(SceneManager.ScreenSize), 25);
			SceneManager.AddObject(label);

			indexes = new GameObject[8];
			for (int i = 0; i < 8; i++) {
				Vector2 size = i >= 4 ? displaySize : gaugeSize;
				float posY = i >= 4 ? (gaugeSize.Y + SPACE) * 2 + (size.Y + SPACE) * ((i - 4) / 2) : (size.Y + SPACE) * (i / 2);

				indexes[i] = new GameObject(size, "", new Vector2(SceneManager.ScreenSize.X - (size.X + SPACE) * (i % 2 + 1), posY, SceneManager.ScreenSize.X, 0, (size.X + SPACE) * (i % 2 + 1)));
				indexes[i].Animator.AddTexture("GAUGE", GetTexture(i));
				indexes[i].Animator.CurrentTexture = "GAUGE";
				indexes[i].IsHidden = true;
				SceneManager.AddObject(indexes[i]);
			}

			backgroundBottom = new GameObject(new Vector2(SceneManager.ScreenSize.X, 200, SceneManager.ScreenSize.X, 0), "", new Vector2(0, SceneManager.ScreenSize.Y - 200, 0, SceneManager.ScreenSize.Y, 0, 200));
			backgroundBottom.Animator.Color = Color.FromArgb(200, 244, 164, 96);
			backgroundBottom.Animator.CurrentTexture = Animator.SOLID_TEXTURE;
			backgroundBottom.IsHidden = true;
			SceneManager.AddObject(backgroundBottom);

			currentSpeech = 0;
			character = new GameObject(new Vector2(150, 150), "", new Vector2(25, SceneManager.ScreenSize.Y - 175, 0, SceneManager.ScreenSize.Y, 0, 175));
			character.IsHidden = true;
			for (int i = 0; i < CHARACTERS_TEXTURES_IDS.Length; i++)
				character.Animator.AddTexture("CHARACTER_" + i, CHARACTERS_TEXTURES_IDS[i]);

			SceneManager.AddObject(character);

			speech = new Label("", 26, Color.White, new Vector2(SceneManager.ScreenSize.X - 200, 200, SceneManager.ScreenSize.X, 0, 200));
			speech.Transform.Position = new Vector2(200, SceneManager.ScreenSize.Y - 175, 0, SceneManager.ScreenSize.Y, 0, 175);
			speech.IsHidden = true;
			SceneManager.AddObject(speech);

			start = SceneManager.Now;
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			// Background color
			gl.ClearColor(51 / 255f, 102 / 255f, 153 / 255f, 0f);    // Ocean

			if (indexes[0].IsHidden && SceneManager.Now.Subtract(start).TotalMilliseconds > idleDurations[LEVEL]) {
				label.IsHidden = true;
				background.IsHidden = backgroundBottom.IsHidden = character.IsHidden = speech.IsHidden = false;
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
							return TAXES_PEOPLE_TEXTURES_IDS[2];
						case 1:
							return TAXES_PEOPLE_TEXTURES_IDS[1];
						default:
							return TAXES_PEOPLE_TEXTURES_IDS[0];
					}
				case 3:
					switch (LEVEL) {
						case 0:
						case 4:
							return TAXES_BUSY_TEXTURES_IDS[2];
						case 1:
							return TAXES_BUSY_TEXTURES_IDS[1];
						case 2:
						case 3:
							return TAXES_BUSY_TEXTURES_IDS[0];
						case 5:
							return TAXES_BUSY_TEXTURES_IDS[3];
						default:
							return TAXES_BUSY_TEXTURES_IDS[4];
					}
				case 4:
					switch (LEVEL) {
						case 1:
						case 6:
							return WORKERS_TEXTURES_IDS[2];
						case 2:
						case 3:
						case 4:
							return WORKERS_TEXTURES_IDS[4];
						case 5:
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
							return SALARY_TEXTURES_IDS[2];
						case 4:
							return SALARY_TEXTURES_IDS[3];
						case 5:
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
