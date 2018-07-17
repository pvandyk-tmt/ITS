using System.Globalization;
using System.Threading;
using System.Windows.Controls;

namespace TMT.iCapture
{
    public class cTextBox : TextBox
    {
        static public string NumericsOnly = "0123456789";
        static public string NumericsOnlySigned = NumericsOnly + "-";
        static public string DecimalOnly = NumericsOnly + ".";
        static public string DecimalOnlySigned = NumericsOnlySigned + ".";
        static public string AllChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-+ ~`!@#$%^&*_=()[]{}<>?/\"|\\;:,âäàåéëèïîìôöòüûùÿý";

        public enum eCaseTypes
        {
            None,
            Lowercase,
            Uppercase,
            Capitalise,
            Numeric
        }

        private char[] mValidChars = AllChars.ToCharArray();
        private eCaseTypes mCaseType = eCaseTypes.None;

        /// <summary>
        /// String conversion to either UPPERCASE, lowercase, Title Case, Numeric or none.
        /// </summary>
        public eCaseTypes pCaseType
        {
            get { return mCaseType; }
            set { mCaseType = value; }
        }

        /// <summary>
        /// Includes only the valid characters that may be typed.
        /// </summary>
        public string pValidChars
        {
            get { return mValidChars.ToString(); }
            set { mValidChars = value.ToCharArray(); }
        }

        public cTextBox()
        {
            this.KeyUp += new System.Windows.Input.KeyEventHandler(cTextBox_KeyUp);
            this.LostFocus += new System.Windows.RoutedEventHandler(cTextBox_LostFocus);
        }

        /// <summary>
        /// Initialise this TextBox.
        /// </summary>
        /// <param name="validChars">Set of valid characters</param>
        /// <param name="length">Maximum length of text allowed</param>
        public void initialise(string validChars, int length, eCaseTypes caseType)
        {
            mValidChars = validChars.ToCharArray();
            mCaseType = caseType;
            this.MaxLength = length;
        }

        /// <summary>
        /// Test if a character is in the valid chars set.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool checkValidChars(char c)
        {
            for (int i = 0; i < mValidChars.Length; i++)
                if (mValidChars[i] == c)
                    return true;

            return false;
        }

        /// <summary>
        /// Check if a string is valid according to the valid chars of this control and the maximum length of this control.
        /// </summary>
        /// <param name="tryString">String to test</param>
        /// <returns>True if valid</returns>
        public bool checkValidChars(string tryString)
        {
            if (string.IsNullOrEmpty(tryString))
                return false;

            int strt = 0;
            char[] txtChars = tryString.ToCharArray();

            for (int i = strt; i < txtChars.Length; i++)
                if (!checkValidChars(txtChars[i]))
                    return false;

            return (tryString.Length <= this.MaxLength);
        }

        private void cTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
                return;

            switch (mCaseType)
            {
                case eCaseTypes.Lowercase:
                    this.Text = this.Text.ToLower();
                    break;
                case eCaseTypes.Uppercase:
                    this.Text = this.Text.ToUpper();
                    break;
                case eCaseTypes.Capitalise:
                    {
                        TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

                        this.Text = textInfo.ToTitleCase(this.Text);
                    }
                    break;
                case eCaseTypes.Numeric:
                    try
                    {
                        decimal.Parse(this.Text);
                    }
                    catch
                    {
                        this.Focus();
                        this.Select(0, this.Text.Length);
                    }
                    break;
            }
        }

        private void cTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Text))
                return;

            int strt = 0;
            int cursorPos = this.SelectionStart;
            char[] txtChars = this.Text.ToCharArray();

            for (int i = strt; i < txtChars.Length; i++)
                if (!checkValidChars(txtChars[i]))
                {
                    cursorPos--;
                    //this.Text = this.Text.Replace(txtChars[i].ToString(), "");
                    this.Text = this.Text.Remove(i, 1);
                    i--;
                    txtChars = this.Text.ToCharArray();
                }

            this.SelectionStart = ((cursorPos > 0) ? cursorPos : 0);
        }
    }
}
