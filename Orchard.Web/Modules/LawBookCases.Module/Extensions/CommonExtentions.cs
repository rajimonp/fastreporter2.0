using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LawBookCases.Module.Extensions
{
    public static class CommonExtentions
    {
        public static string[] GetClients(string title)
        {
            string[] clients = Regex.Split(title, " Vs ", RegexOptions.IgnoreCase);
            if (clients.Count() < 2)
                clients = Regex.Split(title, " V ", RegexOptions.IgnoreCase);
            if (clients.Count() < 2)
                clients = Regex.Split(title, " V. ", RegexOptions.IgnoreCase);
            return clients;
        }
        public static List<string> ExtractCaseHeader(string abstractText)
        {

            if (abstractText == null || !abstractText.Contains("PFR"))
                return new List<string>();

            string html = abstractText;
            //html = "<p style=\"text-align: center;\"><b>		Bayer Intellectual Property Gmbh v. Ajanta Pharma Ltd</b>		</p>\r\n<p style=\"text-align: center;\">		<span style=\"font-weight: 400;\">		HIGH COURT OF DELHI, NEW DELHI		</span></p>\r\n<p style=\"text-align: center;\">		<span style=\"font-weight: 400;\">(Justice R.K. Gauba): Decision dated 04 Jan 2017</span></p>\r\n<p style=\"text-align: center;\"></p>\r\n<p style=\"text-align: center;\"><span style=\"font-weight: 400;\">CS (Comm.) 1648 of 2016, [2017] PFR 1 </span></p>\r\n<p style=\"text-align: center;\"></p>\r\n<p><i><span style=\"font-weight: 400;\">Held, though the non-user of patent in India cannot be set up as a defence to the suit for infringement, upon the self-interest of the patentee being balanced against the larger public interest (para 11)&mdash;</span></i><span style=\"font-weight: 400;\">Franz Xaver Huemer v. New Yash Engineers, AIR 1997 Delhi 79 </span><i><span style=\"font-weight: 400;\">distinguished (para 10)</span></i><span style=\"font-weight: 400;\">.</span></p>\r\n<p><i><span style=\"font-weight: 400;\">Held, though an ex parte ad interim injunction was granted earlier, equity demands that absolute or unconditional temporary injunction be not granted in as much as it would result in the manufacturing activity and the resultant exports of the impugned products of the defendant being ground to a halt resulting possibly in not only loss of employment but revenue to the State as well (para 11)&mdash;The earlier order against making/manufacturing, distribution, offer for sale or sale of the impugned products for purposes of exports stands suspended (para 13). </span></i></p>\r\n<p><i><span style=\"font-weight: 400;\">Held, in the application to set aside the ad interim injunction granted earlier, bearing in mind the element of public interest, balance of convenience, the ex-parte ad-interim injunction granted against the defendant is modified to the effect that the defendant shall stand injuncted from offering for sale, selling or distributing for use or consumption in India the impugned products or any such other product that infringes the subject matter of suit patent Nos. IN 225529 and IN 188419 (Vardenafil) (para 13).</span></i></p>";


            //7*800 5*750

            // use the html agility pack: http://www.codeplex.com/htmlagilitypack
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            List<string> list = new List<string>();
                        
            foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                if (node.Text.Count() > 2)
                {
                    node.Text = new string(node.Text.Where(c => !char.IsControl(c)).ToArray());
                    list.Add(node.Text);
                    if (node.Text.Contains("PFR"))
                        break;
                }
            }
           return list;

        }
    
    }
}