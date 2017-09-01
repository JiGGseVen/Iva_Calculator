using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;
using System.Globalization;
using Iva_Calculator_Prism.Models;

namespace Iva_Calculator_Prism.Services
{
    public class XmlDataService
    {
        private string mainAccountCode = string.Empty;
        private string returnAccountCode = string.Empty;
        private string discountAccountCode = string.Empty;
        private int mainOperationCode = 0; //Sell-> 1 of Buy->0 

        public XmlNode generalLedgerEntries;
        public XmlDocument doc;
        List<XmlNode> filteredList;

        public XmlDataService(Stream fileStream)
        {
            if(fileStream != null)
            {
                doc = new XmlDocument();
                doc.Load(fileStream);

                // Select a list of nodes
                generalLedgerEntries = doc.GetElementsByTagName("GeneralLedgerEntries")[0];
                RetrieveAllValuesOfElement(generalLedgerEntries);
            }
        }

        public void RetrieveAllValuesOfElement(XmlNode node)
        {
            //We are interested in the DebitAmount and CreditAmount attributes values
            //First we are inside the GeneralLedgerEntries tag
            // -> Journal
            // -> Transaction
            // -> Line
            XmlNodeList journalChildNodes = node.ChildNodes;
            filteredList = FindAllJournalTypeNodes(journalChildNodes);
        }

        public string GetMonthOfProcess()
        {
            XmlNode companyAddressNode = doc.GetElementsByTagName("Header")[0];
            var date = Convert.ToDateTime(companyAddressNode.ChildNodes[7].InnerText.ToString());
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
            return monthName.ToUpperInvariant();
        }

        public Product FillProductAmounts(Product product) 
        {
            mainOperationCode = product is BoughtProduct ? 0 : 1;
            
            mainAccountCode = product.ProductAccountCode.ToString();
            returnAccountCode = product.ProductReturnCode.ToString();
            discountAccountCode = product.ProductDiscountCode.ToString();
            
            ProcessTransactions(filteredList, product);
            
            return product;
        }

        private string ConvertStringToUTF8(string myString)
        {
            byte[] bytes = Encoding.Default.GetBytes(myString);
            myString = Encoding.UTF8.GetString(bytes);

            return myString;
        }
    
        public List<XmlNode> FindAllJournalTypeNodes(XmlNodeList journalChildNodes)
        {
            List<XmlNode> journalsOnlyList = new List<XmlNode>();
            foreach (XmlNode currentNode in journalChildNodes)
            {
                if(currentNode.Name == "Journal")
                {
                    journalsOnlyList.Add(currentNode);
                }
            }
            
            return journalsOnlyList;
        }

        private void ProcessTransactions(List<XmlNode> filteredList, Product product)
        {
            //Here the list already contains only Journals

            foreach (XmlNode journalNode in filteredList)
            {
                // Go trough the transaction Lines atributes 
                //XPathNavigator pathNavigator = journalNode.CreateNavigator();
                if (journalNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode transactionNode in journalNode.ChildNodes)
                    {
                        if (transactionNode.Name == "Transaction")
                        {
                            if (transactionNode.ChildNodes.Count > 0)
                            {
                                List<XmlNode> nodeLines = new List<XmlNode>();

                                foreach (XmlNode lineNode in transactionNode.ChildNodes)
                                {
                                    if (lineNode.Name == "Line")
                                    {
                                        nodeLines.Add(lineNode);
                                    }
                                }

                                if(product is BoughtProduct)
                                {
                                    ProcessNodes(nodeLines, (BoughtProduct)product);
                                }
                                else
                                {
                                    ProcessNodes(nodeLines, (SoldProduct)product);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void ProcessNodes(List<XmlNode> nodeLines, SoldProduct product)
        {
            foreach (XmlNode node in nodeLines)
            {
                foreach (XmlNode lineNode in node.ChildNodes)
                {
                    if (lineNode.Name == "AccountID" && lineNode.InnerText == mainAccountCode)
                    {
                        mainOperationCode = 1;
                        XmlNode parent = lineNode.ParentNode;
                        product.SoldAmount += ProcessAccountCode(parent);
                    }
                    else if(lineNode.Name == "AccountID" && lineNode.InnerText == returnAccountCode)
                    {
                        mainOperationCode = 0;
                        XmlNode parent = lineNode.ParentNode;
                        product.ReturnAmount += ProcessAccountCode(parent);
                    }
                    else if(lineNode.Name == "AccountID" && lineNode.InnerText == discountAccountCode)
                    {
                        mainOperationCode = 0;
                        XmlNode parent = lineNode.ParentNode;
                        product.DiscountAmount += ProcessAccountCode(parent);
                    }
                }
            }
        }

        private void ProcessNodes(List<XmlNode> nodeLines, BoughtProduct product)
        {
            foreach (XmlNode node in nodeLines)
            {
                foreach (XmlNode lineNode in node.ChildNodes)
                {
                    if (lineNode.Name == "AccountID" && lineNode.InnerText == mainAccountCode)
                    {
                        XmlNode parent = lineNode.ParentNode;
                        product.BuyAmount += ProcessAccountCode(parent);
                    }
                    else if (lineNode.Name == "AccountID" && lineNode.InnerText == returnAccountCode)
                    {
                        XmlNode parent = lineNode.ParentNode;
                        product.ReturnAmount += ProcessAccountCode(parent);
                    }
                    else if (lineNode.Name == "AccountID" && lineNode.InnerText == discountAccountCode)
                    {
                        XmlNode parent = lineNode.ParentNode;
                        product.DiscountAmount += ProcessAccountCode(parent);
                    }
                }
            }
        }

        private decimal ProcessAccountCode(XmlNode parent)
        {
            decimal resultValue = 0;
            try
            {
                var nodeList = parent.ChildNodes.Cast<XmlNode>();
                if (nodeList != null && nodeList.Count() > 0)
                {
                    if (mainOperationCode == 0)
                    {
                        var Debits = nodeList.Where(x => (x as XmlNode).Name == "DebitAmount");
                        if (Debits != null && Debits.Count() > 0)
                        {
                            if (Debits.First().InnerText != null && Debits.First().InnerText != string.Empty)
                            {
                                string encodedText = string.Empty;
                                encodedText = ConvertStringToUTF8(Debits.First().InnerText);
                                decimal result = Decimal.Parse(encodedText, System.Globalization.NumberStyles.Currency,
                                    System.Globalization.CultureInfo.InvariantCulture);

                                resultValue = result;
                            }
                        }
                    }
                    else if (mainOperationCode == 1)
                    {
                        var Credits = nodeList.Where(x => (x as XmlNode).Name == "CreditAmount");
                        if (Credits != null && Credits.Count() > 0)
                        {
                            if (Credits.First().InnerText != null && Credits.First().InnerText != string.Empty)
                            {
                                string encodedText = string.Empty;
                                encodedText = ConvertStringToUTF8(Credits.First().InnerText);
                                decimal result = Decimal.Parse(encodedText, System.Globalization.NumberStyles.Currency,
                                    System.Globalization.CultureInfo.InvariantCulture);

                                resultValue = result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultValue;
        }
    }
}
