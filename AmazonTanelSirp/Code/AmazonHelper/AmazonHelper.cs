using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using AmazonTanelSirp.ServiceReference1;
using System.ServiceModel;

// Author Tanel Sirp

namespace AmazonTanelSirp.Code.AmazonHelper
{
	public class AmazonHelper
	{
		static string[] _RESPONSE_GROUPS = new string[]{ /*"ItemAttributes",*/ "Images", "EditorialReview", "Small", "OfferSummary" };
		const string _SECRET_KEY = "EkMCWFcbXWIbMn9inPa87p+VJBwaq7U3XEvWMJqI";
		const string _ACCESS_KEY_ID = "AKIAJ24GA7TRBF47QK7A";
		const string _ASSOCIATE_TAG = "fake0d1-20";


		public static AmazonSearchResults Search(string keywords, int resultsRequested, int page)
		{
			AmazonSearchResults results = new AmazonSearchResults();
			results.Items = new List<AmazonItem>();

			List<Item> items = new List<Item>();
			ItemSearchRequest request = GetSearchRequest();
			request.Keywords = keywords;
			
			int itemsCount = 0;//results added to list
			int itemIndex = page * resultsRequested;//item index among all results found by amazon
			int totalResults = 0;//results found by amazon

			bool shouldFinish = false;
			while (itemsCount < resultsRequested && !shouldFinish)
			{
				Debug.WriteLine("querying amazon page " + ((itemIndex / 10) + 1));

				request.ItemPage = Convert.ToString((itemIndex / 10) + 1);
				ItemSearchResponse response = DoAmazonItemSearch(request);

				if (response == null)
				{
					Debug.WriteLine("Response was null");
					break;
				}
				if (response.Items[0].Item == null)
				{
					Debug.WriteLine("No items returned");
					break;
				}
				else
				{
					totalResults = Convert.ToInt32(response.Items[0].TotalResults);
				}

				for(int i = itemIndex % 10; i < 10; i++)
				{
					if (itemIndex >= totalResults || itemIndex >= 50 || itemsCount >= resultsRequested)
					{
						shouldFinish = true;
						break;
					}

					Item item = response.Items[0].Item[i];
					results.Items.Add(CreateAmazonItem(item));
					itemsCount++;
					itemIndex++;
				}
			}

			results.TotalItems = totalResults;
			return results;
		}

		//put results found by amazon into an AmazonItem object
		private static AmazonItem CreateAmazonItem(Item item)
		{
			AmazonItem aitem = new AmazonItem();

			aitem.Title = item. ItemAttributes.Title;

			if (item.EditorialReviews != null)
			{
				aitem.Description = item.EditorialReviews[0].Content;

				if (item.EditorialReviews.Length > 1)
					aitem.Reviews = item.EditorialReviews[1].Content;
			}

			aitem.DetailPageURL = item.DetailPageURL;

			if (item.OfferSummary != null)
			{
                if (item.OfferSummary.LowestNewPrice != null)
                    aitem.LowestNewPrice = item.OfferSummary.LowestNewPrice.FormattedPrice;
                if (item.OfferSummary.LowestUsedPrice != null) 
                    aitem.LowestUsedPrice = item.OfferSummary.LowestUsedPrice.FormattedPrice;
                
			}

            if (item.MediumImage != null)
            {
                aitem.ImageUrl = item.MediumImage.URL;
            }

			return aitem;
		}


        private static ItemSearchResponse DoAmazonItemSearch(ItemSearchRequest request)
		{
			ItemSearchResponse response = null;

            AWSECommerceServicePortTypeClient ecs = GetClient();

			// Create an ItemSearch wrapper
			ItemSearch search = new ItemSearch();
			search.AssociateTag = _ASSOCIATE_TAG;
			search.AWSAccessKeyId = _ACCESS_KEY_ID;

			// Set the request on the search wrapper
			search.Request = new ItemSearchRequest[] { request };

			try
			{
				//Send the request and store the response
				response = ecs.ItemSearch(search);
			}
			catch (NullReferenceException e)
			{
				Debug.WriteLine(e.ToString());
			}
			return response;
		}

		private static AWSECommerceServicePortTypeClient GetClient()
		{
			// Create an instance of the Product Advertising API service
			BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
			binding.MaxReceivedMessageSize = int.MaxValue;

			AWSECommerceServicePortTypeClient ecs = new AWSECommerceServicePortTypeClient(
						binding,
						new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

			// add authentication to the ECS client
			ecs.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(_ACCESS_KEY_ID, _SECRET_KEY));

			return ecs;
		}

		private static ItemSearchRequest GetSearchRequest()
		{
			// Create a request object
			ItemSearchRequest request = new ItemSearchRequest();
			// Fill the request object with request parameters
			request.ResponseGroup = _RESPONSE_GROUPS;

			// Set SearchIndex and Keywords
			request.SearchIndex = "All";

			return request;
		}


	}
}