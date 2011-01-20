using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace CoverFinderGoogle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dictionary<int,String> dic = getSongList();
            for (int i = 0; i < dic.Count; i++)
            {
                KeyValuePair<int, String> elem = dic.ElementAt(i);
                if (!File.Exists(@"E:\covers\ok\" + elem.Key + ".jpg"))
                {
                    //      Console.WriteLine(i + "\t\t" + dic.Keys.ElementAt(i) + "\t" + dic.Values.ElementAt(i));

                    string requestUrl =
                                      string.Format("http://images.google.com/images?" +
                                       "q={0}&start={1}&filter={2}",
                                       elem.Value + " cover", 0,
                                       0);


                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                    string resultPage = string.Empty;
                    using (HttpWebResponse httpWebResponse =
                               (HttpWebResponse)request.GetResponse())
                    {
                        using (Stream responseStream =
                                  httpWebResponse.GetResponseStream())
                        {
                            using (StreamReader reader =
                                     new StreamReader(responseStream))
                            {
                                resultPage = reader.ReadToEnd();
                            }
                        }
                    }


                    int start = resultPage.IndexOf("/imgres?imgurl\\x3") + 18;

                    start = resultPage.IndexOf("/imgres?imgurl\\x3",start+1) + 18;
                    start = resultPage.IndexOf("/imgres?imgurl\\x3", start + 1) + 18;
                    start = resultPage.IndexOf("/imgres?imgurl\\x3", start + 1) + 18;
                    int end = resultPage.IndexOf(".jpg", start) + 4 - start;//\\x26", start) - start;
                    //Console.Write("(" + start + "," + end + ") : ");

                    String imgSrc = resultPage.Substring(start, end);

                    try
                    {
                        WebClient Client = new WebClient();

                        Client.DownloadFileAsync(new Uri(imgSrc), @"E:\covers\" + elem.Key + ".jpg");
                        Console.Write("[OK " + elem.Key + "] ");
                    }
                    catch
                    {
                        Console.Write("[ERROR " + elem.Key + "] ");
                    }

                    Console.WriteLine((dic.Count - i) + "/" + dic.Count + "" + imgSrc + "");

                    
                    
                }
                else
                    Console.WriteLine("[SKIPPED " + elem.Key + "]");
            }
        }

        public Dictionary<int, String> getSongList()
        {
            Dictionary<int, String> dic = new Dictionary<int, string>();
            dic.Add(1, "A-Ha Foot Of The Mountain");
            dic.Add(2, "Agnes Release Me");
            dic.Add(3, "The All-American Rejects Gives You Hell");
            dic.Add(4, "Avril Lavigne Girlfriend");
            dic.Add(5, "Backstreet Boys Incomplete");
            dic.Add(6, "Beyoncé Halo");
            dic.Add(7, "Billy Talent Rusted From The Rain");
            dic.Add(8, "Black Eyed Peas I Gotta Feeling");
            dic.Add(9, "Black Eyed Peas Boom Boom Pow");
            dic.Add(10, "Black Eyed Peas Meet Me Halfway");
            dic.Add(11, "Bob Sinclar Love generation");
            dic.Add(12, "Bon Jovi Have a nice day");
            dic.Add(13, "Boney M. Sunny (remix)");
            dic.Add(14, "Boss Hoss Hey ya");
            dic.Add(15, "The Bosshoss I say a little prayer");
            dic.Add(16, "Buena Mutya Real girl");
            dic.Add(17, "Cassandra Steen Stadt");
            dic.Add(18, "Charlie Winston Like A Hobo");
            dic.Add(19, "Christina Aguilera keeps gettin better");
            dic.Add(20, "Christina Aguilera Hurt");
            dic.Add(21, "Colbie Caillat Fallin' For You");
            dic.Add(22, "Colbie Callait Bubbly");
            dic.Add(23, "Daniel Merriweather Feat. Wale Change");
            dic.Add(24, "Daughtry Over You");
            dic.Add(25, "David Bisbal Silencio");
            dic.Add(26, "David Guetta - Kelly Rowland When Love Takes Over (Feat.Kelly Rowland)");
            dic.Add(27, "Depeche Mode Wrong");
            dic.Add(28, "Die Ärzte Lasse redn");
            dic.Add(29, "Die Ärzte Junge");
            dic.Add(30, "Duffy Mercy");
            dic.Add(31, "Emiliana Torrini Jungle Drum");
            dic.Add(32, "A Fine Frenzy Almost Lover");
            dic.Add(33, "Flo Rida Feat. Ke$ha Right Round");
            dic.Add(34, "Franz Ferdinand No You Girls");
            dic.Add(35, "Gabriella Cilmi Sweet about me");
            dic.Add(36, "Gavin DeGraw Chariot");
            dic.Add(37, "Gnarls Barkley Crazy");
            dic.Add(38, "The Gossip Heavy Cross");
            dic.Add(39, "Green Day Wake me up when september ends");
            dic.Add(40, "Green Day Know Your Enemy");
            dic.Add(41, "Green Day 21 Guns");
            dic.Add(42, "Gwen Stefani The Sweet Escape");
            dic.Add(43, "Herbert Grönemeyer Lied 1 - Stück vom Himmel");
            dic.Add(44, "Ich + Ich Pflaster");
            dic.Add(45, "Ich + Ich So Soll Es Bleiben");
            dic.Add(46, "Ich + Ich Stark");
            dic.Add(47, "Ich + Ich Vom selben Stern");
            dic.Add(48, "Ich + Ich Du Erinnerst Mich An Liebe");
            dic.Add(49, "Jack Johnson Sitting waiting wishing");
            dic.Add(50, "James Blunt Same Mistake");
            dic.Add(51, "James Blunt 1973");
            dic.Add(52, "James Blunt Wisemen");
            dic.Add(53, "James Morrison Please Don't Stop The Rain");
            dic.Add(54, "James Morrison You Give Me Something");
            dic.Add(55, "James Morrison Wonderfull world");
            dic.Add(56, "James Morrison Feat. Nelly Furtado Broken Strings");
            dic.Add(57, "Joana Zimmer I Believe");
            dic.Add(58, "Juanes A dios le pido");
            dic.Add(59, "Juanes La Camisa Negra");
            dic.Add(60, "Juli Dieses Leben");
            dic.Add(61, "Justin Timberlake What goes around comes around");
            dic.Add(62, "K-Maro Femme Like U");
            dic.Add(63, "Kaiser Chiefs Ruby");
            dic.Add(64, "Katy Perry I kissed a girl");
            dic.Add(65, "Katie Melva Nine Million Bicycles");
            dic.Add(66, "Katy Perry Hot N Cold");
            dic.Add(67, "Kelly Clarkson My Life Would Suck Without You");
            dic.Add(68, "Kelly Clarkson Because Of You");
            dic.Add(69, "Kelly Clarkson Walk away");
            dic.Add(70, "Kelly Clarkson Breakaway");
            dic.Add(71, "Kid Rock All Summer Long");
            dic.Add(72, "The Killers Human");
            dic.Add(73, "Kim Wilde You Came (2006 Version)");
            dic.Add(74, "Kings Of Leon Use Somebody");
            dic.Add(75, "Kooks She moves in her own way");
            dic.Add(76, "Kooks Shine On");
            dic.Add(77, "La Roux Bulletproof");
            dic.Add(78, "Lady GaGa Poker Face");
            dic.Add(79, "Laura Pausini & James Blunt Primavera In Anticipo (It Is My Song)");
            dic.Add(80, "Lenka The Show");
            dic.Add(81, "Leona Lewis Bleeding Love");
            dic.Add(82, "Lily Allen Not Fair");
            dic.Add(83, "Lily Allen Smile");
            dic.Add(84, "Linkin Park Shadow of The Day");
            dic.Add(85, "Madonna Jump");
            dic.Add(86, "Madonna Celebration");
            dic.Add(87, "Mando Diao Dance With Somebody");
            dic.Add(88, "Marit Larsen If A Song Could Get Me You");
            dic.Add(89, "Mark Ronson feat. Amy Winehouse Valerie");
            dic.Add(90, "Maroon 5 Makes me wonder");
            dic.Add(91, "Marquess Vayamos companeros");
            dic.Add(92, "Melanie C First Day Of My Life");
            dic.Add(93, "Metro Station Shake It");
            dic.Add(94, "MGMT Kids");
            dic.Add(95, "Mika We Are Golden");
            dic.Add(96, "Mika Big girl");
            dic.Add(97, "Mika Grace Kelly");
            dic.Add(98, "Mika Relax take it easy");
            dic.Add(99, "Milow You Don't Know");
            dic.Add(100, "Milow Ayo Technology");
            dic.Add(101, "Nelly Furtado Say it right");
            dic.Add(102, "Nelly Furtado All good Things (come to an end)");
            dic.Add(103, "Nelly Furtado Maneater");
            dic.Add(104, "Nena Liebe Ist");
            dic.Add(105, "Nick Lachey What´s Left Of Me");
            dic.Add(106, "Nickelback If everyone cared");
            dic.Add(107, "Nickelback Rockstar");
            dic.Add(108, "The Offspring Kristy, Are You Doing Okay?");
            dic.Add(109, "OneRepublic Stop and stare");
            dic.Add(110, "OneRepublic Secrets");
            dic.Add(111, "Paolo Nutini Last Request");
            dic.Add(112, "Peter Cincotti Goodbye Philadelphia");
            dic.Add(113, "Peter Fox Haus Am See");
            dic.Add(114, "Phantom Planet California");
            dic.Add(115, "Pink Funhouse");
            dic.Add(116, "Pink Sober");
            dic.Add(117, "Pink so what");
            dic.Add(118, "Pink Stupid girls");
            dic.Add(120, "Pink Who knew");
            dic.Add(121, "Pink Please Don't Leave Me (Main Version)");
            dic.Add(122, "Pink U + Ur Hand");
            dic.Add(123, "Pink Nobody knows");
            dic.Add(124, "Pitbull I Know You Want Me (Calle Ocho)");
            dic.Add(125, "Placebo For What It's Worth");
            dic.Add(126, "Plain White T's Hey There Delilah");
            dic.Add(127, "Polarkreis 18 allein allein");
            dic.Add(128, "The Pussycat Dolls I Hate This Part");
            dic.Add(129, "Queensberry Too Young");
            dic.Add(130, "Razorlight America");
            dic.Add(131, "Razorlight Wire To Wire");
            dic.Add(132, "Reamonn Tonight");
            dic.Add(133, "Red Hot Chili Peppers Dani california");
            dic.Add(134, "Red Hot Chili Peppers Snow");
            dic.Add(135, "Rihanna Rehab");
            dic.Add(136, "Rihanna Unfaithful");
            dic.Add(137, "Rihanna feat. Ne-Yo Hate That I Love You");
            dic.Add(138, "Rob Thomas Little wonders");
            dic.Add(139, "Rob Thomas Won't go home without you");
            dic.Add(140, "Robbie Williams Rudebox");
            dic.Add(141, "Robbie Williams Tripping");
            dic.Add(142, "Robbie Williams & Pet Shop Boys; Robbie Williams She´s Madonna");
            dic.Add(143, "Roger Cicero Fraun regiern die welt");
            dic.Add(144, "Roger Cicero Zieh die schuh aus");
            dic.Add(145, "Rooney when did your heart go missin");
            dic.Add(146, "Rosenstolz Ich Bin Ich (Wir Sind Wir)");
            dic.Add(147, "Rosenstolz Ich geh in Flammen auf");
            dic.Add(148, "Sarah Connor Son of a preacher man");
            dic.Add(149, "Sarah Connor From Zero To Hero");
            dic.Add(150, "Sasha Lucky day");
            dic.Add(151, "Sasha Slowly");
            dic.Add(153, "Scissor Sisters I Don't Feel Like Dancin'");
            dic.Add(154, "Seal Amazing");
            dic.Add(155, "Sebastian Hämer Sommer unseres lebens");
            dic.Add(156, "Sercio Mendes& Black Eyed Peas Mas que nada");
            dic.Add(157, "Shaggy feel the rush");
            dic.Add(158, "Shakira She Wolf");
            dic.Add(160, "Shakira Feat. Carlos Santana Illegal");
            dic.Add(161, "Shakira Feat. Wyclef Jean Hips Don´t Lie");
            dic.Add(162, "Shinedown Second Chance (Album Version)");
            dic.Add(163, "Silbermond Irgendwas Bleibt");
            dic.Add(165, "Silbermond Das Beste");
            dic.Add(166, "Snow Patrol Chasing Cars");
            dic.Add(167, "Snow Patrol Crack The Shutters");
            dic.Add(169, "Sportfreunde Stiller");
            dic.Add(170, "Sportfreunde Stiller Ein Kompliment (unplugged)");
            dic.Add(171, "Stacie Orrico I´m Not Missing You");
            dic.Add(172, "Stanfour For All Lovers");
            dic.Add(173, "Stefanie Heinzmann No One (Can Ever Change My Mind)");
            dic.Add(174, "Stefanie Heinzmann  My Man Is A Mean Man");
            dic.Add(175, "Sugababes About you now");
            dic.Add(176, "Sugababes Push The Button");
            dic.Add(177, "Sunrise Avenue Fairytale gone bad");
            dic.Add(178, "Sunrise Avenue The Whole Story");
            dic.Add(179, "Texas Lightning No no never");
            dic.Add(180, "Timbaland Apologize");
            dic.Add(181, "The Ting Tings Shut Up And Let Me Go");
            dic.Add(182, "Ville Valo Natalia Avelon Summer Wine");
            dic.Add(183, "Wir sind Helden Nur ein Wort");
            dic.Add(184, "Xavier Naidoo Dieser Weg");
            dic.Add(185, "Yael Naim New Soul");

            return dic;

        }
    }
}
