using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Core.Xamarin.Controls {

	public partial class BadgeTextView : ContentView {

		public static readonly BindableProperty BadgeBackgroundColorProperty = BindableProperty.Create(nameof(BadgeBackgroundColor), typeof(Color), typeof(BadgeTextView),
			Color.White);
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(BadgeTextView),
			Color.Black);
		public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgeTextView),
			"", propertyChanged: TextChanged);
		public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(BadgeTextView),
			FontAttributes.None, propertyChanged: FontAttributesChanged);
		public static readonly BindableProperty BadgePaddingProperty = BindableProperty.Create(nameof(BadgePadding), typeof(float), typeof(BadgeTextView),
			(float)2);

		///<summary>The background color of the badge.</summary>
		public Color BadgeBackgroundColor {
			get { return (Color)GetValue(BadgeBackgroundColorProperty); }
			set { SetValue(BadgeBackgroundColorProperty, value); }
		}

		///<summary>The color of the text.</summary>
		public Color TextColor {
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		///<summary>The contents of the text.</summary>
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		///<summary>The font attributes for this text.</summary>
		public FontAttributes FontAttributes {
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		///<summary>The padding within the frame of the badge.</summary>
		public float BadgePadding {
			get { return (float)GetValue(BadgePaddingProperty); }
			set { SetValue(BadgePaddingProperty, value); }
		}

		public BadgeTextView() {
			InitializeComponent();
			_textLabel.SizeChanged += (o,e) => {
				AdjustFrame(this);
			};
		}

		private static void TextChanged(BindableObject bindable, object oldValue, object newValue) {
			if(!(bindable is BadgeTextView badgeView)) {
				return;
			}
			AdjustFrame(badgeView);
		}

		private static void FontAttributesChanged(BindableObject bindable, object oldValue, object newValue) {
			if(!(bindable is BadgeTextView badgeView)) {
				return;
			}
			AdjustFrame(badgeView);
		}

		///<summary>Adjusts the size of the frame to react to the labels size. If the label is small, will produce a badge. If the label is long,
		///will produce a Rounded badge around the </summary>
		private static void AdjustFrame(BadgeTextView badgeView) {
			SizeRequest request = badgeView._textLabel.Measure(double.PositiveInfinity, double.PositiveInfinity);
			double frameHeight = request.Request.Height + badgeView.BadgePadding * 2;
			double frameWidth = Math.Max(request.Request.Width + badgeView.BadgePadding * 2, frameHeight);
			float cornerRadius = (float)frameHeight / 2;
			badgeView._circleFrame.WidthRequest = frameWidth;
			badgeView._circleFrame.HeightRequest = frameHeight;
			badgeView._circleFrame.CornerRadius = cornerRadius;
		}
	}

}
