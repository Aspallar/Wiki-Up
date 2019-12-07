/* Source originally obtained from https://github.com/Nimgoble/WPFTextBoxAutoComplete 
 * under the following license
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014 Nimgoble
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.

 * Modified by Aspallar 2019
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    public static class AutoCompleteBehavior
    {
        private static readonly TextChangedEventHandler onTextChanged = new TextChangedEventHandler(OnTextChanged);
        private static readonly KeyEventHandler onKeyDown = new KeyEventHandler(OnPreviewKeyDown);
        
		/// <summary>
		/// The collection to search for matches from.
		/// </summary>
        public static readonly DependencyProperty AutoCompleteItemsSource =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteItemsSource",
                typeof(IEnumerable<String>),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(null, OnAutoCompleteItemsSource)
            );

		/// <summary>
		/// Whether or not to ignore case when searching for matches.
		/// </summary>
		public static readonly DependencyProperty AutoCompleteStringComparison =
			DependencyProperty.RegisterAttached
			(
				"AutoCompleteStringComparison",
				typeof(StringComparison),
				typeof(AutoCompleteBehavior),
				new UIPropertyMetadata(StringComparison.Ordinal)
			);

        /// <summary>
		/// What string should indicate that we should start giving auto-completion suggestions.  For example: @
        /// If this is null or empty, auto-completion suggestions will begin at the beginning of the textbox's text.
		/// </summary>
		public static readonly DependencyProperty AutoCompleteIndicator =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteIndicator",
                typeof(String),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(String.Empty)
            );

        #region Items Source
        public static IEnumerable<String> GetAutoCompleteItemsSource(DependencyObject obj)
        {
            return obj.GetValue(AutoCompleteItemsSource) as IEnumerable<String>;
        }

        public static void SetAutoCompleteItemsSource(DependencyObject obj, IEnumerable<String> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }

        private static void OnAutoCompleteItemsSource(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TextBox tb))
                return;

            //If we're being removed, remove the callbacks
            //Remove our old handler, regardless of if we have a new one.
            tb.TextChanged -= onTextChanged;
            tb.PreviewKeyDown -= onKeyDown;
            if (e.NewValue != null)
            {
                //New source.  Add the callbacks
                tb.TextChanged += onTextChanged;
                tb.PreviewKeyDown += onKeyDown;
            }
        }
		#endregion

		#region String Comparison
		public static StringComparison GetAutoCompleteStringComparison(DependencyObject obj) 
		{
			return (StringComparison)obj.GetValue(AutoCompleteStringComparison);
		}

		public static void SetAutoCompleteStringComparison(DependencyObject obj, StringComparison value) 
		{
			obj.SetValue(AutoCompleteStringComparison, value);
		}
        #endregion

        #region Indicator
        public static String GetAutoCompleteIndicator(DependencyObject obj)
        {
            return (String)obj.GetValue(AutoCompleteIndicator);
        }

        public static void SetAutoCompleteIndicator(DependencyObject obj, String value)
        {
            obj.SetValue(AutoCompleteIndicator, value);
        }
        #endregion

        /// <summary>
        /// Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (!(e.OriginalSource is TextBox tb))
                return;

            //If we pressed enter and if the selected text goes all the way to the end, move our caret position to the end
            if (tb.SelectionLength > 0 && (tb.SelectionStart + tb.SelectionLength == tb.Text.Length))
            {
                tb.SelectionStart = tb.CaretIndex = tb.Text.Length;
                tb.SelectionLength = 0;
            }
        }

		/// <summary>
		/// Search for auto-completion suggestions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Any(x => x.RemovedLength > 0) && !e.Changes.Any(x => x.AddedLength > 0))
                return;

            if (!(e.OriginalSource is TextBox tb))
                return;

            IEnumerable<String> values = GetAutoCompleteItemsSource(tb);
            string searchTarget = tb.Text;

            //No reason to search if we don't have any values or there's nothing to search for.
            if (values == null || string.IsNullOrEmpty(searchTarget))
                return;

            string indicator = GetAutoCompleteIndicator(tb);
            int startIndex = 0; 

            //If we have a trigger string, make sure that it has been typed before
            //giving auto-completion suggestions.
            if (!string.IsNullOrEmpty(indicator))
            {
                startIndex = tb.Text.LastIndexOf(indicator);
                //If we haven't typed the trigger string, then don't do anything.
                if (startIndex == -1)
                    return;

                startIndex += indicator.Length;
                searchTarget = searchTarget.Substring(startIndex, searchTarget.Length - startIndex);
            }

            //If we don't have anything after the trigger string, return.
            if (string.IsNullOrEmpty(searchTarget))
                return;

            int searchTargetLength = searchTarget.Length;

			StringComparison compareType = GetAutoCompleteStringComparison(tb);

            var match = values.Where(x => x.StartsWith(searchTarget, compareType))
                .Select(x => x.Substring(searchTargetLength, x.Length - searchTargetLength))
                .FirstOrDefault();

            if (string.IsNullOrEmpty(match))
				return;

            int matchStart = startIndex + searchTargetLength;
            tb.TextChanged -= onTextChanged;
            tb.Text += match;
            tb.CaretIndex = matchStart;
            tb.SelectionStart = matchStart;
            tb.SelectionLength = tb.Text.Length - startIndex;
            tb.TextChanged += onTextChanged;
        }
    }
}
