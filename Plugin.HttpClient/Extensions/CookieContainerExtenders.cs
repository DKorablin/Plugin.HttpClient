using System;
using System.Collections;
using System.Net;

namespace Plugin.HttpClient
{
	internal static class CookieContainerExtenders
	{
		public static void ParseCookieHeader(this CookieContainer container, Uri host, String[] cookies)
		{
			foreach(String cookie in cookies)
			{
				String[] keyValue = cookie.Split(new Char[] { '=' }, 2);
				container.Add(host, new Cookie(keyValue[0], keyValue[1]));
			}
		}

		public static void Parse(this CookieCollection cc, String host, String cookiesString)
		{//TODO: Not working
			/*var collection = new CookieCollection();
			collection.Parse("http://test", "last_visit=1714151445449%3A%3A1714162245449; adtech_uid=486e8796-8083-4a3e-8b9e-025207b340f1%3Aexist.ru; top100_id=t1.1518885.1472629986.1629992513635; t3_sid_1518885=s1.1636055206.1739894140442.1739894163072.711.26; _ga=GA1.2.1767676230.1690368507; _ga_ZZE8MRMW4L=GS1.2.1708774658.26.1.1708774749.60.0.0; a=D18DE0DB101CB93999379D73E74EE21B29CF7629F79BF56A02A7EA22839D4322649699784FADBD14D54B7AA1CF41630AEFA6F6C6831CB1AB8DDE82B118D4B004974E6D59A0EB83D43B6BB9F3255DDFA6781C8799BDAB321165C2DA431020BAEEDC3C3E7AE8330BC9C577DBDA5C8D62527E139F288ECE007B6BD31D92DAEF9F22145317016666935FC03C0559DE8D2C9A84AE94F50BD31626E8711FF960FB97848CFE6C70560B62C6B34EC30649FDA06ECD2CFFDCBAD5C09F8D6291A16D700FC67345F3B8E9EE512356A0A50B58AE189B457E3893CA38E6EC0BC5A7CD0F2112945E4D0571AA9302F880956A3A263E59ABAD2E1356311E0238BF197B8CDF4E9B39E436C66018A17443AACF2216D7CFFAFFD2663E512CE2D4755D860CEEAA8B0DC60CA96C21FFC8C0468BA00071137AD91AC060B484D73CF8B2EEBACCA2E6F68C607E7DC60E2AA90DFFE45EA02CB7A5CF87A86B7B6D73D2B977560E3AF3A144A60BD7BCE86E9F62043EED2BEA2E83D13D1F6FDA3CBA3DA6CBDDB2E331CA5D904A8FB09B0D0B5BF3E77BB5AD315DFE18DD984263C0FEF3EB059EFFB8CCC9859C3A5408AAB15D29BB7216E2D07AAAF9315063BA783C5D3AF49C4A67EB0F15EEC4FA2DDD27900F3406F13F947CA75B4F5D5772902C3707F3253CF1228A045000254854FC3D1C335B77951FCD0A9D559017D0733D38466E8F94FB847CDDC0F89522189FCCA98E4B5634E5AD2227A060006AAA8D01539725E5E24F0BF5C47333C445F8B10CEC29AAEAB7F901A037A53000DC6AD9F3F7503FF1B9B72596EFE9A002540ABEF80A82115EE326460F8E611B0A7F0D6453F6D0DBC16AEF30A66BEB71F0D8B281B713A7CAC3BA4B7FB5503A98173EA3B1494F702ED934D20766D7CB4625DE5C3E33ABA550DD784B0D352C3608C46A5D616716AF04E224EFCC76A560D7B7738B2F171A46F811B8FDD6096D5E15B546A2BDE8C3069EF5E29BA19BF41225458403DF4F2E878606F5C15C494DF098F3E53FE7D426322F2DFEFE6E8D21171BCB3CB2BE6A034E85C40482153192C3DCCC9D0D9565AE4EE7DFD75B82FCF545154E5383FA906CA8EFF990BF66C9DB7A14A8F96AE7DD596E3794BC60475FD86C4381E963F1B26E396F5A66A89223352E5835EC2039EBED7E73DA435C845A3CB0FD44A9BE738A8C82AF385527CCDE56892262FD474290D1ABEF3328322E6959BDED3547BD92C9F80FCD9F4682E07625675AB28CCE91");
			var cc = new CookieContainer();
			cc.Add(new Uri("http://test"), collection);*/

			ArrayList al = ConvertCookieHeaderToArrayList(cookiesString);

			Int32 alcount = al.Count;
			String strEachCook;
			String[] strEachCookParts;
			for(Int32 i = 0; i < alcount; i++)
			{
				strEachCook = al[i].ToString();
				strEachCookParts = strEachCook.Split(';');
				Int32 intEachCookPartsCount = strEachCookParts.Length;
				String strCNameAndCValue = String.Empty;
				String strPNameAndPValue = String.Empty;
				String strDNameAndDValue = String.Empty;
				String[] NameValuePairTemp;
				Cookie cookTemp = new Cookie();

				for(Int32 j = 0; j < intEachCookPartsCount; j++)
				{
					if(j == 0)
					{
						strCNameAndCValue = strEachCookParts[j];
						if(strCNameAndCValue != String.Empty)
						{
							Int32 firstEqual = strCNameAndCValue.IndexOf("=");
							String firstName = strCNameAndCValue.Substring(0, firstEqual);
							String allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
							cookTemp.Name = firstName;
							cookTemp.Value = allValue;
						}
						continue;
					}
					if(strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						strPNameAndPValue = strEachCookParts[j];
						if(strPNameAndPValue != String.Empty)
						{
							NameValuePairTemp = strPNameAndPValue.Split('=');
							cookTemp.Path = NameValuePairTemp[1] != String.Empty
								? NameValuePairTemp[1]
								: "/";
						}
						continue;
					}

					if(strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						strPNameAndPValue = strEachCookParts[j];
						if(strPNameAndPValue != String.Empty)
						{
							NameValuePairTemp = strPNameAndPValue.Split('=');

							cookTemp.Domain = NameValuePairTemp[1] != String.Empty
								? NameValuePairTemp[1]
								: host;
						}
						continue;
					}
				}

				if(cookTemp.Path == String.Empty)
					cookTemp.Path = "/";
				if(cookTemp.Domain == String.Empty)
					cookTemp.Domain = host;

				cc.Add(cookTemp);
			}
		}

		private static ArrayList ConvertCookieHeaderToArrayList(String cookiesString)
		{
			cookiesString = cookiesString.Replace("\r", "");
			cookiesString = cookiesString.Replace("\n", "");
			String[] strCookTemp = cookiesString.Split(',');
			ArrayList al = new ArrayList();
			Int32 i = 0;
			Int32 n = strCookTemp.Length;

			while(i < n)
			{
				if(strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
				{
					al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
					i = i + 1;
				} else
					al.Add(strCookTemp[i]);
				i = i + 1;
			}
			return al;
		}
	}
}