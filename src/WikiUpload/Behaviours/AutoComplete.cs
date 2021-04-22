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

 * Modified by Aspallar 2019-2020
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    public static class AutoComplete
    {
        #region ItemsSource Property
        /// <summary>
        /// The collection to search for matches from.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached
            (
                "ItemsSource",
                typeof(IEnumerable<string>),
                typeof(AutoComplete),
                new UIPropertyMetadata(null, OnItemsSourceChanged)
            );

        public static IEnumerable<string> GetItemsSource(DependencyObject obj)
        {
            return obj.GetValue(ItemsSourceProperty) as IEnumerable<string>;
        }

        public static void SetItemsSource(DependencyObject obj, IEnumerable<string> value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            //If we're being removed, remove the callbacks
            //Remove our old handler, regardless of if we have a new one.
            tb.TextChanged -= TextBox_TextChanged;
            tb.PreviewKeyDown -= TextBox_PreviewKeyDown;
            if (e.NewValue != null)
            {
                //New source.  Add the callbacks
                tb.TextChanged += TextBox_TextChanged;
                tb.PreviewKeyDown += TextBox_PreviewKeyDown;
            }
        }
        #endregion

        #region StringComparison Property
        /// <summary>
        /// Whether or not to ignore case when searching for matches.
        /// </summary>
        public static readonly DependencyProperty StringComparisonProperty =
			DependencyProperty.RegisterAttached
			(
				"StringComparison",
				typeof(StringComparison),
				typeof(AutoComplete),
				new UIPropertyMetadata(StringComparison.Ordinal)
			);

        public static StringComparison GetStringComparison(DependencyObject obj)
        {
            return (StringComparison)obj.GetValue(StringComparisonProperty);
        }

        public static void SetStringComparison(DependencyObject obj, StringComparison value)
        {
            obj.SetValue(StringComparisonProperty, value);
        }

        #endregion

        #region Indicator Property
        /// <summary>
        /// What string should indicate that we should start giving auto-completion suggestions.  For example: @
        /// If this is null or empty, auto-completion suggestions will begin at the beginning of the textbox's text.
        /// </summary>
        public static readonly DependencyProperty IndicatorProperty =
            DependencyProperty.RegisterAttached
            (
                "Indicator",
                typeof(string),
                typeof(AutoComplete),
                new UIPropertyMetadata(string.Empty)
            );

        public static string GetIndicator(DependencyObject obj)
        {
            return (string)obj.GetValue(IndicatorProperty);
        }

        public static void SetIndicator(DependencyObject obj, string value)
        {
            obj.SetValue(IndicatorProperty, value);
        }
        #endregion

        #region TextBox event handlers
        /// <summary>
        /// Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (e.OriginalSource is not TextBox tb)
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
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.Any(x => x.RemovedLength > 0) && !e.Changes.Any(x => x.AddedLength > 0))
                return;

            if (e.OriginalSource is not TextBox tb)
                return;

            var values = GetItemsSource(tb);
            var searchTarget = tb.Text;

            //No reason to search if we don't have any values or there's nothing to search for.
            if (values == null || string.IsNullOrEmpty(searchTarget))
                return;

            var indicator = GetIndicator(tb);
            var startIndex = 0; 

            //If we have a trigger string, make sure that it has been typed before
            //giving auto-completion suggestions.
            if (!string.IsNullOrEmpty(indicator))
            {
                startIndex = tb.Text.LastIndexOf(indicator);
                //If we haven't typed the trigger string, then don't do anything.
                if (startIndex == -1)
                    return;

                startIndex += indicator.Length;
                searchTarget = searchTarget[startIndex..];
            }

            //If we don't have anything after the trigger string, return.
            if (string.IsNullOrEmpty(searchTarget))
                return;

            var searchTargetLength = searchTarget.Length;

			var compareType = GetStringComparison(tb);

            var match = values.Where(x => x.StartsWith(searchTarget, compareType))
                .Select(x => x[searchTargetLength..])
                .FirstOrDefault();

            if (string.IsNullOrEmpty(match))
				return;

            var matchStart = startIndex + searchTargetLength;
            tb.TextChanged -= TextBox_TextChanged;
            tb.Text += match;
            tb.CaretIndex = matchStart;
            tb.SelectionStart = matchStart;
            tb.SelectionLength = tb.Text.Length - startIndex;
            tb.TextChanged += TextBox_TextChanged;
        }

        #endregion
    }
}
