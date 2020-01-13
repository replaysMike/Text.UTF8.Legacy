using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Text.UTF8.Legacy
{
    //
    // Data table for encoding classes.  Used by System.Text.Encoding.
    // This class contains two hashtables to allow System.Text.Encoding
    // to retrieve the data item either by codepage value or by webName.
    //
    internal static partial class EncodingTable
    {
        private static readonly Hashtable _nameToCodePage = Hashtable.Synchronized(new Hashtable(StringComparer.OrdinalIgnoreCase));
        private static CodePageDataItem[] _codePageToCodePageData;

        /*=================================GetCodePageFromName==========================
        **Action: Given a encoding name, return the correct code page number for this encoding.
        **Returns: The code page for the encoding.
        **Arguments:
        **  name    the name of the encoding
        **Exceptions:
        **  ArgumentNullException if name is null.
        **  internalGetCodePageFromName will throw ArgumentException if name is not a valid encoding name.
        ============================================================================*/

        internal static int GetCodePageFromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            object codePageObj = _nameToCodePage[name];

            if (codePageObj != null)
            {
                return (int)codePageObj;
            }

            int codePage = InternalGetCodePageFromName(name);

            _nameToCodePage[name] = codePage;

            return codePage;
        }

        // Find the data item by binary searching the table.
        private static int InternalGetCodePageFromName(string name)
        {
            int left = 0;
            int right = _encodingNameIndices.Length - 2;
            int index;
            int result;

            //Debug.Assert(s_encodingNameIndices.Length == s_codePagesByName.Length + 1);
            //Debug.Assert(s_encodingNameIndices[^1] == s_encodingNames.Length);

            ReadOnlySpan<char> invariantName = name.ToLowerInvariant().AsSpan();

            // Binary search the array until we have only a couple of elements left and then
            // just walk those elements.
            while ((right - left) > 3)
            {
                index = ((right - left) / 2) + left;

                Debug.Assert(index < _encodingNameIndices.Length - 1);
                //result = string.CompareOrdinal(name.ToLowerInvariant(), s_encodingNames.AsSpan(s_encodingNameIndices[index], s_encodingNameIndices[index + 1] - s_encodingNameIndices[index]));
                result = string.CompareOrdinal(name.ToLowerInvariant(), EncodingNames);

                if (result == 0)
                {
                    // We found the item, return the associated codePage.
                    return _codePagesByName[index];
                }
                else if (result < 0)
                {
                    // The name that we're looking for is less than our current index.
                    right = index;
                }
                else
                {
                    // The name that we're looking for is greater than our current index
                    left = index;
                }
            }

            // Walk the remaining elements (it'll be 3 or fewer).
            for (; left <= right; left++)
            {
                Debug.Assert(left < _encodingNameIndices.Length - 1);
                //if (string.CompareOrdinal(invariantName, s_encodingNames.AsSpan(s_encodingNameIndices[left], s_encodingNameIndices[left + 1] - s_encodingNameIndices[left])) == 0)
                if (string.CompareOrdinal(name.ToLowerInvariant(), EncodingNames) == 0)
                {
                    return _codePagesByName[left];
                }
            }

            // The encoding name is not valid.
            throw new ArgumentException("Encoding not supported", nameof(name));
        }

        // Return a list of all EncodingInfo objects describing all of our encodings
        internal static EncodingInfo[] GetEncodings()
        {
            var arrayEncodingInfo = new EncodingInfo[_mappedCodePages.Length];

            for (int i = 0; i < _mappedCodePages.Length; i++)
            {
                arrayEncodingInfo[i] = new EncodingInfo(
                    _mappedCodePages[i],
                    string.Empty,
                    //s_webNames[s_webNameIndices[i]..s_webNameIndices[i + 1]],
                    GetDisplayName(_mappedCodePages[i], i)
                    );
            }

            return arrayEncodingInfo;
        }

        internal static CodePageDataItem GetCodePageDataItem(int codePage)
        {
            if (_codePageToCodePageData == null)
            {
                Interlocked.CompareExchange<CodePageDataItem[]>(ref _codePageToCodePageData, new CodePageDataItem[_mappedCodePages.Length], null);
            }

            // Keep in sync with s_mappedCodePages
            int index;
            switch (codePage)
            {
                case 1200: // utf-16
                    index = 0;
                    break;
                case 1201: // utf-16be
                    index = 1;
                    break;
                case 12000: // utf-32
                    index = 2;
                    break;
                case 12001: // utf-32be
                    index = 3;
                    break;
                case 20127: // us-ascii
                    index = 4;
                    break;
                case 28591: // iso-8859-1
                    index = 5;
                    break;
                case 65000: // utf-7
                    index = 6;
                    break;
                case 65001: // utf-8
                    index = 7;
                    break;
                default:
                    return null;
            }

            CodePageDataItem data = _codePageToCodePageData[index];
            if (data == null)
            {
                Interlocked.CompareExchange<CodePageDataItem>(ref _codePageToCodePageData[index], InternalGetCodePageDataItem(codePage, index), null);
                data = _codePageToCodePageData[index];
            }

            return data;
        }

        private static CodePageDataItem InternalGetCodePageDataItem(int codePage, int index)
        {
            int uiFamilyCodePage = _uiFamilyCodePages[index];
            //string webName = s_webNames[s_webNameIndices[index]..s_webNameIndices[index + 1]];
            string webName = "webName not configured";
            // All supported code pages have identical header names, and body names.
            string headerName = webName;
            string bodyName = webName;
            string displayName = GetDisplayName(codePage, index);
            uint flags = _flags[index];

            return new CodePageDataItem(uiFamilyCodePage, webName, headerName, bodyName, displayName, flags);
        }

        private static string GetDisplayName(int codePage, int englishNameIndex)
        {
            var displayName = "Globalization_cp_" + codePage.ToString();
            if (string.IsNullOrEmpty(displayName))
                //displayName = s_englishNames[s_englishNameIndices[englishNameIndex]..s_englishNameIndices[englishNameIndex + 1]];
                displayName = "Globalization not configured";

            return displayName;
        }
    }
}
