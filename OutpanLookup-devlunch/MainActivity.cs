using System;
using System.IO;
using System.Net;
using Android.App;
using Android.Content;
using System.Text;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json;
using OutPanApiGet;

namespace OutpanLookupdevlunch
{
	[Activity (Label = "OutpanLookup-devlunch", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Wire up our buttons
			Button btnGo = FindViewById<Button> (Resource.Id.btnGo);	
			btnGo.Click += (o, e) => {
				//Toast.MakeText(this,"Working", ToastLength.Short);
				btnGet_Click (o, e);
			};

			var credentials = FindViewById<EditText> (Resource.Id.ApiKey);
			credentials.Text = "47294cbe8bf4ddd9229772e881cad634:";

			Button btnSet = FindViewById<Button> (Resource.Id.btnSetAPI);
			btnSet.Click += (o, e) => {
				SetCredetntials (credentials.Text);
			};

		}

		private void btnGet_Click (object sender, EventArgs e)
		{
			var credentials = GetCredentials ();
			if (string.IsNullOrEmpty (credentials)) {
				Toast.MakeText (this, "Please set your credentials first!", ToastLength.Long);
				return;
			}

			var ean = GetEan ();
			if (string.IsNullOrEmpty (ean)) {
				Toast.MakeText (this, "Please set a EAN first", ToastLength.Long);
				return;
			}

			var suffix = GetSuffix ();

			var url = SetUrl (ean, suffix);

			WebRequest myReq = WebRequest.Create (url);
			myReq.Headers ["Authorization"] = "Basic " + Convert.ToBase64String (Encoding.ASCII.GetBytes ("47294cbe8bf4ddd9229772e881cad634:"));
			try {
				WebResponse wr = myReq.GetResponse ();
				Stream receiveStream = wr.GetResponseStream ();
			
				if (receiveStream != null) {
					StreamReader reader = new StreamReader (receiveStream, Encoding.UTF8);
					string content = reader.ReadToEnd ();
					switch (suffix) {
					case "":
						var allResult = JsonConvert.DeserializeObject<OutpanApiAllModel> (content);
						var formatted = JsonConvert.SerializeObject (allResult, Formatting.Indented);
						ClearText ();
						SetTextOutPut (formatted);
						break;
					case "attributes":
						var attrResult = JsonConvert.DeserializeObject<OutpanApiAttributesModel> (content);
						var attrformatted = JsonConvert.SerializeObject (attrResult, Formatting.Indented);
						ClearText ();
						SetTextOutPut (attrformatted);
						break;
					case "images":
						var imageResult = JsonConvert.DeserializeObject<OutpanApiImageModel> (content);
						var imgformatted = JsonConvert.SerializeObject (imageResult, Formatting.Indented);
						ClearText ();
						SetTextOutPut (imgformatted);
						break;
					case "name":
						var nameResult = JsonConvert.DeserializeObject<OutpanApiNameModel> (content);
						var nameformatted = JsonConvert.SerializeObject (nameResult, Formatting.Indented);
						ClearText ();
						SetTextOutPut (nameformatted);
						break;
					case "videos":
						var videoResult = JsonConvert.DeserializeObject<OutpanApiVideosModel> (content);
						var videoformatted = JsonConvert.SerializeObject (videoResult, Formatting.Indented);
						ClearText ();
						SetTextOutPut (videoformatted);
						break;
					}

				}
			} catch (Exception ex) {
				var err = ex;
			}
		}

		public string SetUrl (string gtin, string suffix)
		{
			var url = "https://api.outpan.com/v1/products/" + gtin + "/" + suffix;
			return url;
		}

		public string GetEan ()
		{
			var txtEanNumber = FindViewById<EditText> (Resource.Id.EanNumber);
			return txtEanNumber.Text.Trim ();
		}

		public string GetSuffix ()
		{
			Spinner typeSpinner = (Spinner)FindViewById (Resource.Id.spinRequestType);
			var spinnerText = typeSpinner.SelectedItem.ToString ();
			if (spinnerText == "All") {
				return "";
			}
			return spinnerText.ToLower ();
		}

		public void SetCredetntials (string credentials)
		{
			var prefs = Application.Context.GetSharedPreferences ("OutpanLookup-devlunch", FileCreationMode.Private);
			var prefEditor = prefs.Edit ();
			prefEditor.PutString ("userCredentials", credentials);
			prefEditor.Commit ();
			prefEditor.Apply ();	
		}

		public string GetCredentials ()
		{
			var prefs = Application.Context.GetSharedPreferences ("OutpanLookup-devlunch", FileCreationMode.Private);
			var test = prefs.GetString ("userCredentials", null);
			return prefs.GetString ("userCredentials", null);
		}

		public void ClearText ()
		{
			TextView et = (TextView)FindViewById (Resource.Id.TextOutPut);
			et.Text = "";
		}

		public void SetTextOutPut (string formatted)
		{
			TextView et = FindViewById<TextView> (Resource.Id.TextOutPut);
			et.Text = formatted;
		}
	}
}


