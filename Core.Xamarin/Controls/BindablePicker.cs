using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace Core.Xamarin.Controls {

	///<summary>A picker that supports binding. Allows you to bind to multiple properties within a class as well as retain selection when raising property changed.</summary>
	public class BindablePicker : Picker {

		public static new readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(BindablePicker), null,
			propertyChanged: OnItemsSourceChanged);
		public static new readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(BindablePicker), null,
			BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);
		public static readonly BindableProperty SelectedValueProperty = BindableProperty.Create(nameof(SelectedValue), typeof(object), typeof(BindablePicker), null,
			BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);
		public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(BindablePicker), null,
			BindingMode.OneWay);

		///<summary>Disables calls to events. Prevents infinite loops.</summary>
		private bool _areNestedCallsDisabled;
		///<summary>An event that is fired when the selected item changes.</summary>
		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

		///<summary>Returns if a display member path has been set.</summary>
		public bool HasDisplayMemberPath {
			get {
				return !string.IsNullOrEmpty(DisplayMemberPath);
			}
		}

		///<summary>Returns if a selected value path has been set.</summary>
		public bool HasSelectedValuePath {
			get {
				return !string.IsNullOrEmpty(SelectedValuePath);
			}
		}

		///<summary>Indicates if a display member converter was set.</summary>
		public bool HasDisplayMemberConverter {
			get {
				return DisplayMemberConverter!=null;
			}
		}

		///<summary>The property name of the string that should be used as the display path.</summary>
		public string DisplayMemberPath { get; set; }

		///<summary>The property name of the selected value path. This is the path to the property within the SelectedItem.</summary>
		public string SelectedValuePath { get; set; }

		///<summary>A converter for the display value of the item. Will be passed the item to convert, or if a DisplayMemberPath is set, will pass the value of the display member.</summary>
		public IValueConverter DisplayMemberConverter { get; set; }

		///<summary>The item source for the picker.</summary>
		public new IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		///<summary>The selected item. This will match the object in the ItemsSource.</summary>
		public new object SelectedItem {
			get {
				return GetValue(SelectedItemProperty);
			}
			set {
				if(SelectedItem!=value) {
					SetValue(SelectedItemProperty, value);
					InternalSelectedItemChanged();
				}
			}
		}

		///<summary>The selected value of the picker. This is the second property of the SelectedItem that can be bound to another property.</summary>
		public object SelectedValue {
			get {
				return GetValue(SelectedValueProperty);
			}
			set {
				SetValue(SelectedValueProperty, value);
				InternalSelectedValueChanged();
			}
		}

		///<summary>A command that will fire when an item is selected.</summary>
		public ICommand ItemSelectedCommand {
			get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
			set { SetValue(ItemSelectedCommandProperty, value); }
		}

		public BindablePicker() {
			SelectedIndexChanged+=OnSelectedIndexChanged;
		}

		///<summary>Occurs when the item source changes.</summary>
		private void InstanceOnItemsSourceChanged(object oldValue, object newValue) {
			_areNestedCallsDisabled=true;
			Items.Clear();
			if(oldValue is INotifyCollectionChanged oldValueCollection) {
				oldValueCollection.CollectionChanged-=ItemsSource_CollectionChanged;
			}
			if(newValue is INotifyCollectionChanged newValueCollection) {
				newValueCollection.CollectionChanged+=ItemsSource_CollectionChanged;
			}
			if(newValue!=null) {
				foreach(object item in (IEnumerable)newValue) {
					Items.Add(GetDisplayStringValue(item));
				}
				SelectedIndex=-1;
				_areNestedCallsDisabled=false;
				if(SelectedItem!=null) {
					InternalSelectedItemChanged();
				}
				else if((HasDisplayMemberPath||HasDisplayMemberConverter)&&SelectedValue!=null) {
					InternalSelectedValueChanged();
				}
			}
			else {
				_areNestedCallsDisabled=true;
				SelectedIndex=-1;
				SelectedItem=null;
				SelectedValue=null;
				_areNestedCallsDisabled=false;
			}
		}

		///<summary>Runs when the SelectedItem is changed. Will update the selected value when this occurs</summary>
		private void InternalSelectedItemChanged() {
			if(_areNestedCallsDisabled) {
				return;
			}
			int selectedIndex = -1;
			object selectedValue = null;
			if(ItemsSource!=null) {
				int index = 0;
				foreach(object item in ItemsSource) {
					if(item!=null&&item.Equals(SelectedItem)) {
						selectedIndex=index;
						if(HasSelectedValuePath) {
							selectedValue=GetSelectedValue(item);
						}
						break;
					}
					index++;
				}
			}
			_areNestedCallsDisabled=true;
			SelectedValue=selectedValue;
			SelectedIndex=selectedIndex;
			_areNestedCallsDisabled=false;
		}

		///<summary>Occurs when the selected value changes.</summary>
		private void InternalSelectedValueChanged() {
			if(_areNestedCallsDisabled) {
				return;
			}
			if(!HasSelectedValuePath) {
				return;
			}
			int selectedIndex = -1;
			object selectedItem = null;
			if(ItemsSource!=null) {
				int index = 0;
				foreach(object item in ItemsSource) {
					if(item!=null) {
						object selectedValueItem = GetSelectedValue(item);
						//Preserve selection if the selected value is equivalent.
						if(Equals(selectedValueItem, SelectedValue)) {
							selectedIndex=index;
							selectedItem=item;
							break;
						}
					}
					index++;
				}
			}
			_areNestedCallsDisabled=true;
			SelectedItem=selectedItem;
			SelectedIndex=selectedIndex;
			_areNestedCallsDisabled=false;
		}

		private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.Action==NotifyCollectionChangedAction.Add) {
				foreach(object item in e.NewItems) {
					Items.Add(GetDisplayStringValue(item));
				}
			}
			else if(e.Action==NotifyCollectionChangedAction.Remove) {
				foreach(object item in e.OldItems) {
					Items.Remove(GetDisplayStringValue(item));
				}
			}
			else if(e.Action==NotifyCollectionChangedAction.Replace) {
				foreach(object item in e.NewItems) {
					int index = Items.IndexOf(GetDisplayStringValue(item));
					if(index>-1) {
						Items[index]=item.ToString();
					}
				}
			}
			else if(e.Action==NotifyCollectionChangedAction.Reset) {
				Items.Clear();
				_areNestedCallsDisabled=true;
				SelectedItem=null;
				SelectedIndex=-1;
				SelectedValue=null;
				_areNestedCallsDisabled=false;
			}
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e) {
			if(_areNestedCallsDisabled) {
				return;
			}
			_areNestedCallsDisabled = true;
			if(SelectedIndex < 0 || ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext()) {
				if(SelectedIndex != -1) {
					SelectedIndex = -1;
				}
				SelectedItem = null;
				SelectedValue = null;
				_areNestedCallsDisabled = false;
				return;
			}
			int index = 0;
			foreach(object item in ItemsSource) {
				if(index == SelectedIndex) {
					SelectedItem = item;
					if(HasSelectedValuePath) {
						SelectedValue = GetSelectedValue(item);
					}
					break;
				}
				index++;
			}
			ItemSelectedCommand?.Execute(null);
			_areNestedCallsDisabled=false;
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue) {
			if(Equals(newValue, null)&&Equals(oldValue, null)) {
				return;
			}
			BindablePicker picker = (BindablePicker)bindable;
			picker.InstanceOnItemsSourceChanged(oldValue, newValue);
		}

		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue) {
			BindablePicker boundPicker = (BindablePicker)bindable;
			boundPicker.InvalidateMeasure();
			boundPicker.ItemSelected?.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
			boundPicker.InternalSelectedItemChanged();
		}

		private static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue) {
			BindablePicker boundPicker = (BindablePicker)bindable;
			boundPicker.InternalSelectedValueChanged();
		}

		///<summary>For the given item, returns the string that will display on the picker. Takes into account DisplayMemberPath and the display converter.</summary>
		private string GetDisplayStringValue(object item) {
			object itemToConvert = item;
			//If they have a display member, we will use that to display and/or convert.
			if(HasDisplayMemberPath) {
				Type type = item.GetType();
				PropertyInfo prop = type.GetRuntimeProperty(DisplayMemberPath);
				if(prop == null) {
					throw new ApplicationException($"Invalid {nameof(DisplayMemberPath)}: {SelectedValuePath} for type: {type.Name}");
				}
				itemToConvert=prop.GetValue(item);
			}
			if(!HasDisplayMemberConverter) {
				return itemToConvert?.ToString()??"";
			}
			object converterRetVal = DisplayMemberConverter.Convert(itemToConvert, typeof(string), null, CultureInfo.CurrentCulture);
			if(!(converterRetVal is string retValStr)) {
				throw new ApplicationException($"Converter: {DisplayMemberConverter.GetType().Name} did not return a string.");
			}
			return retValStr;
		}

		///<summary>For the given item, returns the selected value.</summary>
		private object GetSelectedValue(object item) {
			if(item == null) {
				return null;
			}
			if(!HasSelectedValuePath) {
				throw new ApplicationException($@"Calling GetSelectedValue without a valie {nameof(SelectedValuePath)}.");
			}
			Type type = item.GetType();
			PropertyInfo prop = type.GetRuntimeProperty(SelectedValuePath);
			if(prop == null) {
				throw new ApplicationException($"Invalid {nameof(SelectedValuePath)}: {SelectedValuePath} for type: {type.Name}");
			}
			return prop.GetValue(item);
		}
	}
}