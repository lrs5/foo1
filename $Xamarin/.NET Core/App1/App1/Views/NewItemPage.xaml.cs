﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using App1.Models;

namespace App1.Views {
	public partial class NewItemPage : ContentPage {
		public Item Item { get; set; }

		public NewItemPage() {
			InitializeComponent();

			Item = new Item {
				Text = "Item name",
				Description = "This is an item description."
			};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e) {
			MessagingCenter.Send(this, "AddItem", Item);
			await Navigation.PopModalAsync();
		}

		async void Cancel_Clicked(object sender, EventArgs e) {
			await Navigation.PopModalAsync();
		}
	}
}