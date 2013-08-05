using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmazonTanelSirp.Code.AmazonHelper
{
	public class AmazonItem
	{
		public string Title { get; set; }
		public string Description { get; set; }
        public string Reviews { get; set; }
        public string LowestNewPrice { get; set; }
        public string LowestUsedPrice { get; set; }
		public string DetailPageURL { get; set; }
		public string ImageUrl { get; set; }
	}
}