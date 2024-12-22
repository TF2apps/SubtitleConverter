using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Winforms
{
    public partial class Form1 : Form
    {
        #region Variables/Objects

        SoundPlayer _gameSound = new SoundPlayer(Properties.Resources.mixkit_retro_game_notification_212);
        SoundPlayer _failSound = new SoundPlayer(Properties.Resources.mixkit_wrong_answer_fail_notification_946);

        SemaphoreSlim _lock;

        string _filePath = "";

        #endregion Variables/Objects

        #region Set Form Colours

        private void SetColours()
        {
            #region DeepL

            if (String.IsNullOrWhiteSpace(textBoxDeeplApiKey.Text))
            {
                textBoxDeeplApiKey.BackColor = Color.LightSkyBlue;
            }
            else
            {
                textBoxDeeplApiKey.BackColor = Color.White;
            }
            if (String.IsNullOrWhiteSpace(_filePath))
            {
                buttonDeeplSelectFile.BackColor = Color.LightSkyBlue;
            }
            else
            {
                buttonDeeplSelectFile.BackColor = Color.White;
            }
            if (!String.IsNullOrWhiteSpace(textBoxDeeplApiKey.Text) && !String.IsNullOrWhiteSpace(_filePath))
            {
                buttonDeeplTranslate.BackColor = Color.LightSkyBlue;
                buttonDeeplTranslate.Enabled = true;
            }
            else
            {
                buttonDeeplTranslate.BackColor = Color.White;
                buttonDeeplTranslate.Enabled = false;
            }

            #endregion DeepL

            #region DeepL Pro

            if (String.IsNullOrWhiteSpace(textBoxDeeplProApiKey.Text))
            {
                textBoxDeeplProApiKey.BackColor = Color.Plum;
            }
            else
            {
                textBoxDeeplProApiKey.BackColor = Color.White;
            }
            if (String.IsNullOrWhiteSpace(_filePath))
            {
                buttonDeeplProSelectFile.BackColor = Color.Plum;
            }
            else
            {
                buttonDeeplProSelectFile.BackColor = Color.White;
            }
            if (!String.IsNullOrWhiteSpace(textBoxDeeplProApiKey.Text) && !String.IsNullOrWhiteSpace(_filePath))
            {
                buttonDeeplProTranslate.BackColor = Color.Plum;
                buttonDeeplProTranslate.Enabled = true;
            }
            else
            {
                buttonDeeplProTranslate.BackColor = Color.White;
                buttonDeeplProTranslate.Enabled = false;
            }

            #endregion DeepL Pro

            #region Google

            if (String.IsNullOrWhiteSpace(textBoxGoogleApiKey.Text))
            {
                textBoxGoogleApiKey.BackColor = Color.LightSalmon;
            }
            else
            {
                textBoxGoogleApiKey.BackColor = Color.White;
            }
            if (String.IsNullOrWhiteSpace(_filePath))
            {
                buttonGoogleSelectFile.BackColor = Color.LightSalmon;
            }
            else
            {
                buttonGoogleSelectFile.BackColor = Color.White;
            }
            if (!String.IsNullOrWhiteSpace(textBoxGoogleApiKey.Text) && !String.IsNullOrWhiteSpace(_filePath))
            {
                buttonGoogleTranslate.BackColor = Color.LightSalmon;
                buttonGoogleTranslate.Enabled = true;
            }
            else
            {
                buttonGoogleTranslate.BackColor = Color.White;
                buttonGoogleTranslate.Enabled = false;
            }

            #endregion Google
        }

        #endregion Set Form Colours

        public Form1()
        {
            InitializeComponent();

            _lock = new SemaphoreSlim(1);

            #region Hide Tabs

            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            tabControl1.BackColor = this.BackColor;

            #endregion Hide Tabs

            #region Set Tab

            SelectTab((int)Properties.Settings.Default["tab"]);

            #endregion Set Tab

            #region Set API Keys

            // DeepL

            textBoxDeeplApiKey.Text = (string)Properties.Settings.Default["apiKeyDeepl"];

            // DeepL Pro

            textBoxDeeplProApiKey.Text = (string)Properties.Settings.Default["apiKeyDeeplPro"];

            // Google

            textBoxGoogleApiKey.Text = (string)Properties.Settings.Default["apiKeyGoogle"];

            #endregion Set API Keys

            #region Set ComboBox Selections

            // DeepL

            comboBoxDeeplInput.SelectedIndex = (int)Properties.Settings.Default["inputLangDeepl"];
            comboBoxDeeplOutput.SelectedIndex = (int)Properties.Settings.Default["outputLangDeepl"];

            // DeepL Pro

            comboBoxDeeplProInput.SelectedIndex = (int)Properties.Settings.Default["inputLangDeeplPro"];
            comboBoxDeeplProOutput.SelectedIndex = (int)Properties.Settings.Default["outputLangDeeplPro"];

            // Google

            comboBoxGoogleInput.SelectedIndex = (int)Properties.Settings.Default["inputLangGoogle"];
            comboBoxGoogleOutput.SelectedIndex = (int)Properties.Settings.Default["outputLangGoogle"];

            #endregion Set ComboBox Selections

            #region Set Colours

            UnlockAllControls();

            #endregion Set Colours
        }

        #region -- Translator Selection Button Events --

        #region DeepL

        private void buttonDeeplDeeplPro_Click(object sender, EventArgs e)
        {
            SelectTab(1);
        }

        private void buttonDeeplGoogle_Click(object sender, EventArgs e)
        {
            SelectTab(2);
        }

        #endregion Deepl

        #region DeeplPro

        private void buttonDeeplProDeepl_Click(object sender, EventArgs e)
        {
            SelectTab(0);
        }

        private void buttonDeeplProGoogle_Click(object sender, EventArgs e)
        {
            SelectTab(2);
        }

        #endregion DeeplPro

        #region Google

        private void buttonGoogleDeepl_Click(object sender, EventArgs e)
        {
            SelectTab(0);
        }

        private void buttonGoogleDeeplPro_Click(object sender, EventArgs e)
        {
            SelectTab(1);
        }

        #endregion Google

        private void SelectTab(int selectedTab)
        {
            tabControl1.SelectedTab = tabControl1.TabPages[selectedTab];

            Properties.Settings.Default["tab"] = selectedTab;
            Properties.Settings.Default.Save();
        }

        #endregion -- Translator Selection Button Events

        #region -- API Key Changed Events --

        #region DeepL

        private void textBoxDeeplApiKey_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["apiKeyDeepl"] = textBoxDeeplApiKey.Text;
            Properties.Settings.Default.Save();

            SetColours();
        }

        #endregion DeepL

        #region DeepL Pro

        private void textBoxDeeplProApiKey_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["apiKeyDeeplPro"] = textBoxDeeplProApiKey.Text;
            Properties.Settings.Default.Save();

            SetColours();
        }

        #endregion DeepL Pro

        #region Google

        private void textBoxGoogleApiKey_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["apiKeyGoogle"] = textBoxGoogleApiKey.Text;
            Properties.Settings.Default.Save();

            SetColours();
        }

        #endregion Google

        #endregion -- API Key Changed Events --

        #region -- API Key Show/Hide Events --

        private void pictureBoxDeeplKey_Click(object sender, EventArgs e)
        {
            textBoxDeeplApiKey.UseSystemPasswordChar = !textBoxDeeplApiKey.UseSystemPasswordChar;
        }

        private void pictureBoxDeeplProKey_Click(object sender, EventArgs e)
        {
            textBoxDeeplProApiKey.UseSystemPasswordChar = !textBoxDeeplProApiKey.UseSystemPasswordChar;
        }

        private void pictureBoxGoogleKey_Click(object sender, EventArgs e)
        {
            textBoxGoogleApiKey.UseSystemPasswordChar = !textBoxGoogleApiKey.UseSystemPasswordChar;
        }

        #endregion -- API Key Show/Hide Events --

        #region -- Select File -- ***

        private void buttonDeeplSelectFile_Click(object sender, EventArgs e)
        {
            SelectFile();
        }

        private void buttonDeeplProSelectFile_Click(object sender, EventArgs e)
        {
            SelectFile();
        }

        private void buttonGoogleSelectFile_Click(object sender, EventArgs e)
        {
            SelectFile();
        }

        private void SelectFile()
        {
            string initialDirectory = (string)Properties.Settings.Default["dir"];

            // Use a default directory if the saved directory is invalid
            if (string.IsNullOrWhiteSpace(initialDirectory) || !Directory.Exists(initialDirectory))
            {
                initialDirectory = "C:\\";
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = initialDirectory;
                ofd.Filter = "Subtitle Files (*.vtt)|*.vtt";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _filePath = ofd.FileName;

                    string selectedDirectory = Path.GetDirectoryName(_filePath);
                    Properties.Settings.Default["dir"] = selectedDirectory;
                    Properties.Settings.Default.Save();
                }
            }

            string file = Path.GetFileName(_filePath);

            labelDeeplSelectedFile.Text = file;
            labelDeeplSelectedFile.Font = new Font(labelDeeplSelectedFile.Font, FontStyle.Regular);

            labelDeeplProSelectedFile.Text = file;
            labelDeeplProSelectedFile.Font = new Font(labelDeeplProSelectedFile.Font, FontStyle.Regular);

            labelGoogleSelectedFile.Text = file;
            labelGoogleSelectedFile.Font = new Font(labelGoogleSelectedFile.Font, FontStyle.Regular);

            SetColours();
        }


        #endregion -- Select File --

        #region -- Selected Language Changed Events --

        #region Deepl

        private void comboBoxDeeplInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["inputLangDeepl"] = comboBoxDeeplInput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void comboBoxDeeplOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["outputLangDeepl"] = comboBoxDeeplOutput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        #endregion Deepl

        #region Deepl Pro

        private void comboBoxDeeplProInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["inputLangDeeplPro"] = comboBoxDeeplProInput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void comboBoxDeeplProOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["outputLangDeeplPro"] = comboBoxDeeplProOutput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        #endregion Deepl Pro

        #region Google

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["inputLangGoogle"] = comboBoxGoogleInput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["outputLangGoogle"] = comboBoxGoogleOutput.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        #endregion Google

        #endregion -- Selected Language Changed Events --

        #region -- Translate Button Click Events -- ****

        private void LockAllControls()
        {
            #region DeepL

            buttonDeeplGoogle.Enabled = false;
            buttonDeeplDeeplPro.Enabled = false;

            textBoxDeeplApiKey.Enabled = false;

            buttonDeeplSelectFile.Enabled = false;

            comboBoxDeeplInput.Enabled = false;
            comboBoxDeeplOutput.Enabled = false;

            buttonDeeplTranslate.BackColor = Color.White;
            buttonDeeplTranslate.Enabled = false;

            pictureBoxDeeplLoading.Visible = true;

            #endregion DeepL

            #region DeepL Pro

            buttonDeeplProGoogle.Enabled = false;
            buttonDeeplProDeepl.Enabled = false;

            textBoxDeeplProApiKey.Enabled = false;

            buttonDeeplProSelectFile.Enabled = false;

            comboBoxDeeplProInput.Enabled = false;
            comboBoxDeeplProOutput.Enabled = false;

            buttonDeeplProTranslate.BackColor = Color.White;
            buttonDeeplProTranslate.Enabled = false;

            pictureBoxDeeplProLoading.Visible = true;

            #endregion DeepL Pro

            #region Google

            buttonGoogleDeepl.Enabled = false;
            buttonGoogleDeeplPro.Enabled = false;

            textBoxGoogleApiKey.Enabled = false;

            buttonGoogleSelectFile.Enabled = false;

            comboBoxGoogleInput.Enabled = false;
            comboBoxGoogleOutput.Enabled = false;

            buttonGoogleTranslate.BackColor = Color.White;
            buttonGoogleTranslate.Enabled = false;

            pictureBoxGoogleLoading.Visible = true;

            #endregion Google
        }

        private void UnlockAllControls()
        {
            #region DeepL

            buttonDeeplGoogle.Enabled = true;
            buttonDeeplDeeplPro.Enabled = true;

            textBoxDeeplApiKey.Enabled = true;

            buttonDeeplSelectFile.Enabled = true;

            comboBoxDeeplInput.Enabled = true;
            comboBoxDeeplOutput.Enabled = true;

            buttonDeeplTranslate.Enabled = true;

            pictureBoxDeeplLoading.Visible = false;

            #endregion DeepL

            #region DeepL Pro

            buttonDeeplProGoogle.Enabled = true;
            buttonDeeplProDeepl.Enabled = true;

            textBoxDeeplProApiKey.Enabled = true;

            buttonDeeplProSelectFile.Enabled = true;

            comboBoxDeeplProInput.Enabled = true;
            comboBoxDeeplProOutput.Enabled = true;

            buttonDeeplProTranslate.Enabled = true;

            pictureBoxDeeplProLoading.Visible = false;

            #endregion DeepL Pro

            #region Google

            buttonGoogleDeepl.Enabled = true;
            buttonGoogleDeeplPro.Enabled = true;

            textBoxGoogleApiKey.Enabled = true;

            buttonGoogleSelectFile.Enabled = true;

            comboBoxGoogleInput.Enabled = true;
            comboBoxGoogleOutput.Enabled = true;

            buttonGoogleTranslate.Enabled = true;

            pictureBoxGoogleLoading.Visible = false;

            #endregion Google

            SetColours();
        }

        private async void buttonDeeplTranslate_Click(object sender, EventArgs e)
        {
            using (_lock)
            {
                try
                {
                    LockAllControls();

                    string filePath = _filePath;

                    if (String.IsNullOrWhiteSpace(filePath))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Select a .vtt file", "No input file selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    if (String.IsNullOrWhiteSpace((string)Properties.Settings.Default["apiKeyDeepl"]))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Enter your deepL API key", "No API key entered", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    #region Simple Method

                    // Step 1 - Turn the VTT into a list of subtitles
                    var subtitles = VttToListOfSubtitle(filePath);

                    // Step 2 - Directly translate subtitles into translated subtitles
                    var translatedSubtitles = await TranslateSubtitlesDeepl(subtitles);

                    // Step 3 - Create VTT files

                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string directory = Path.GetDirectoryName(filePath);
                    string extension = Path.GetExtension(filePath);

                    string languageInput = GetDeeplLanguageCode(comboBoxDeeplInput.Text);
                    if (String.IsNullOrWhiteSpace(languageInput)) languageInput = "xx";
                    string languageOutput = GetDeeplLanguageCode(comboBoxDeeplOutput.Text);

                    string translatedFilePath = Path.Combine(directory, $"{fileName} {languageOutput}{extension}");
                    string combinedFilePath = Path.Combine(directory, $"{fileName} {languageInput}-{languageOutput}{extension}");

                    CreateVttFile(translatedSubtitles, translatedFilePath);
                    CreateVttFileCombined(translatedSubtitles, combinedFilePath);

                    #endregion Simple Method

                    _filePath = "";
                    labelDeeplSelectedFile.Text = "No File selected";
                    labelDeeplSelectedFile.Font = new Font(labelDeeplSelectedFile.Font, FontStyle.Italic);

                    labelDeeplProSelectedFile.Text = "No File selected";
                    labelDeeplProSelectedFile.Font = new Font(labelDeeplProSelectedFile.Font, FontStyle.Italic);

                    labelGoogleSelectedFile.Text = "No File selected";
                    labelGoogleSelectedFile.Font = new Font(labelGoogleSelectedFile.Font, FontStyle.Italic);
                }
                catch (IOException)
                {
                    _failSound.Play();

                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("An error occurred processing the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    UnlockAllControls();

                    _gameSound.Play();
                }
            }
        }

        private async void buttonDeeplProTranslate_Click(object sender, EventArgs e)
        {
            using (_lock)
            {
                try
                {
                    string filePath = _filePath;

                    if (String.IsNullOrWhiteSpace(filePath))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Select a .vtt file", "No input file selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    if (String.IsNullOrWhiteSpace((string)Properties.Settings.Default["apiKeyDeeplPro"]))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Enter your deepL Pro API key", "No API key entered", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    #region Simple Method

                    // Step 1 - Turn the VTT into a list of subtitles
                    var subtitles = VttToListOfSubtitle(filePath);

                    // Step 2 - Directly translate subtitles into translated subtitles
                    var translatedSubtitles = await TranslateSubtitlesDeeplPro(subtitles);

                    // Step 3 - Create VTT files

                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string directory = Path.GetDirectoryName(filePath);
                    string extension = Path.GetExtension(filePath);

                    string languageInput = GetDeeplLanguageCode(comboBoxDeeplProInput.Text);
                    if (String.IsNullOrWhiteSpace(languageInput)) languageInput = "xx";
                    string languageOutput = GetDeeplLanguageCode(comboBoxDeeplProOutput.Text);

                    string translatedFilePath = Path.Combine(directory, $"{fileName} {languageOutput}{extension}");
                    string combinedFilePath = Path.Combine(directory, $"{fileName} {languageInput}-{languageOutput}{extension}");

                    CreateVttFile(translatedSubtitles, translatedFilePath);
                    CreateVttFileCombined(translatedSubtitles, combinedFilePath);

                    #endregion Simple Method

                    _filePath = "";
                    labelDeeplSelectedFile.Text = "No File selected";
                    labelDeeplSelectedFile.Font = new Font(labelDeeplSelectedFile.Font, FontStyle.Italic);

                    labelDeeplProSelectedFile.Text = "No File selected";
                    labelDeeplProSelectedFile.Font = new Font(labelDeeplProSelectedFile.Font, FontStyle.Italic);

                    labelGoogleSelectedFile.Text = "No File selected";
                    labelGoogleSelectedFile.Font = new Font(labelGoogleSelectedFile.Font, FontStyle.Italic);
                }
                catch (IOException)
                {
                    _failSound.Play();

                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("An error occurred processing the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    UnlockAllControls();

                    _gameSound.Play();
                }
            }
        }

        private async void buttonGoogleTranslate_Click(object sender, EventArgs e)
        {
            using (_lock)
            {
                try
                {
                    string filePath = _filePath;

                    if (String.IsNullOrWhiteSpace(filePath))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Select a .vtt file", "No input file selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    if (String.IsNullOrWhiteSpace((string)Properties.Settings.Default["apiKeyGoogle"]))
                    {
                        using (new CenterWinDialog(this))
                        {
                            MessageBox.Show("Enter your Google API key", "No API key entered", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        return;
                    }

                    #region Simple Method

                    // Step 1 - Turn the VTT into a list of subtitles
                    var subtitles = VttToListOfSubtitle(filePath);

                    // Step 2 - Directly translate subtitles into translated subtitles
                    var translatedSubtitles = await TranslateSubtitlesGoogle(subtitles);

                    // Step 3 - Create VTT files

                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string directory = Path.GetDirectoryName(filePath);
                    string extension = Path.GetExtension(filePath);

                    string languageInput = GetGoogleLanguageCode(comboBoxGoogleInput.Text);
                    if (String.IsNullOrWhiteSpace(languageInput)) languageInput = "xx";
                    string languageOutput = GetGoogleLanguageCode(comboBoxGoogleOutput.Text);

                    string translatedFilePath = Path.Combine(directory, $"{fileName} {languageOutput}{extension}");
                    string combinedFilePath = Path.Combine(directory, $"{fileName} {languageInput}-{languageOutput}{extension}");

                    CreateVttFile(translatedSubtitles, translatedFilePath);
                    CreateVttFileCombined(translatedSubtitles, combinedFilePath);

                    #endregion Simple Method

                    _filePath = "";
                    labelDeeplSelectedFile.Text = "No File selected";
                    labelDeeplSelectedFile.Font = new Font(labelDeeplSelectedFile.Font, FontStyle.Italic);

                    labelDeeplProSelectedFile.Text = "No File selected";
                    labelDeeplProSelectedFile.Font = new Font(labelDeeplProSelectedFile.Font, FontStyle.Italic);

                    labelGoogleSelectedFile.Text = "No File selected";
                    labelGoogleSelectedFile.Font = new Font(labelGoogleSelectedFile.Font, FontStyle.Italic);
                }
                catch (IOException)
                {
                    _failSound.Play();

                    using (new CenterWinDialog(this))
                    {
                        MessageBox.Show("An error occurred processing the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    UnlockAllControls();

                    _gameSound.Play();
                }
            }
        }

        #endregion -- Translate Button Click Events --

        #region String Methods

        private string ReplaceSemicolonWithComma(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
            {
                return csvLine; // Return the original string if it's null or empty
            }

            return csvLine.Replace(';', ',');
        }

        private bool IsEndOfSentence(string text)
        {
            return Regex.IsMatch(text, @"[.!?]$");
        }

        #endregion String Methods

        #region API Methods

        #region DeepL Methods

        private string GetDeeplLanguageCode(string language)
        {
            switch (language)
            {
                case "?":
                    return "";
                case "Arabic":
                    return "ar";
                case "Bulgarian":
                    return "bg";
                case "Chinese":
                    return "zh";
                case "Chinese (Simplified)":
                    return "zh-hans";
                case "Chinese (Traditional)":
                    return "zh-hant";
                case "Czech":
                    return "cs";
                case "Danish":
                    return "da";
                case "Dutch":
                    return "nl";
                case "English":
                    return "en";
                case "English (British)":
                    return "en-gb";
                case "English (American)":
                    return "en-us";
                case "Estonian":
                    return "et";
                case "Finnish":
                    return "fi";
                case "French":
                    return "fr";
                case "German":
                    return "de";
                case "Greek":
                    return "el";
                case "Hungarian":
                    return "hu";
                case "Indonesian":
                    return "id";
                case "Italian":
                    return "it";
                case "Japanese":
                    return "ja";
                case "Korean":
                    return "ko";
                case "Latvian":
                    return "lv";
                case "Lithuanian":
                    return "lt";
                case "Norwegian Bokmal":
                    return "nb";
                case "Polish":
                    return "pl";
                case "Portuguese":
                    return "pt";
                case "Portuguese (Brazilian)":
                    return "pt-br";
                case "Portuguese (European)":
                    return "pt-pt";
                case "Romanian":
                    return "ro";
                case "Russian":
                    return "ru";
                case "Slovak":
                    return "sk";
                case "Slovenian":
                    return "sl";
                case "Spanish":
                    return "es";
                case "Swedish":
                    return "sv";
                case "Turkish":
                    return "tr";
                case "Ukrainian":
                    return "uk";
                default:
                    return "en";
            }
        }

        private async Task<string> TranslateTextDeepl(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return sentence;
            }

            try
            {
                string inputLanguage = GetDeeplLanguageCode(comboBoxDeeplInput.Text);
                string outputLanguage = GetDeeplLanguageCode(comboBoxDeeplOutput.Text);

                using (HttpClient client = new HttpClient())
                {
                    string requestUri = $"https://api-free.deepl.com/v2/translate";

                    //var content = new FormUrlEncodedContent(new[]
                    //{
                    //new KeyValuePair<string, string>("auth_key", Properties.Settings.Default.apiKeyDeepl),
                    //new KeyValuePair<string, string>("text", sentence),
                    //new KeyValuePair<string, string>("source_lang", inputLanguage),
                    //new KeyValuePair<string, string>("target_lang", outputLanguage)
                    //});

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("auth_key", Properties.Settings.Default.apiKeyDeepl),
                        new KeyValuePair<string, string>("text", sentence),
                        new KeyValuePair<string, string>("target_lang", outputLanguage)
                    };

                    if (!string.IsNullOrWhiteSpace(inputLanguage))
                    {
                        parameters.Add(new KeyValuePair<string, string>("source_lang", inputLanguage));
                    }

                    var content = new FormUrlEncodedContent(parameters);

                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    string translatedText = json["translations"][0]["text"].ToString();

                    return translatedText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Translation error: " + ex.Message);
                return "Error translating text";
            }
        }

        private async Task<List<TranslatedSubtitle>> TranslateSubtitlesDeepl(List<Subtitle> subtitles)
        {
            List<TranslatedSubtitle> translatedSubtitles = new List<TranslatedSubtitle>();

            foreach (var subtitle in subtitles)
            {
                string translation = await TranslateTextDeepl(subtitle.Text);

                translatedSubtitles.Add(new TranslatedSubtitle { Start = subtitle.Start, End = subtitle.End, Text = subtitle.Text, TranslatedText = translation });
            }
            return translatedSubtitles;
        }

        #endregion DeepL Methods

        #region DeepL Pro Methods

        private async Task<string> TranslateTextDeeplPro(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return sentence;
            }

            try
            {
                string inputLanguage = GetDeeplLanguageCode(comboBoxDeeplProInput.Text);
                string outputLanguage = GetDeeplLanguageCode(comboBoxDeeplProOutput.Text);

                using (HttpClient client = new HttpClient())
                {
                    string requestUri = $"https://api.deepl.com/v2/translate";

                    //var content = new FormUrlEncodedContent(new[]
                    //{
                    //new KeyValuePair<string, string>("auth_key", Properties.Settings.Default.apiKeyDeeplPro),
                    //new KeyValuePair<string, string>("text", sentence),
                    //new KeyValuePair<string, string>("source_lang", inputLanguage),
                    //new KeyValuePair<string, string>("target_lang", outputLanguage)
                    //}

                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("auth_key", Properties.Settings.Default.apiKeyDeeplPro),
                        new KeyValuePair<string, string>("text", sentence),
                        new KeyValuePair<string, string>("target_lang", outputLanguage)
                    };

                    if (!string.IsNullOrWhiteSpace(inputLanguage))
                    {
                        parameters.Add(new KeyValuePair<string, string>("source_lang", inputLanguage));
                    }

                    var content = new FormUrlEncodedContent(parameters);

                    HttpResponseMessage response = await client.PostAsync(requestUri, content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    string translatedText = json["translations"][0]["text"].ToString();

                    return translatedText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Translation error: " + ex.Message);
                return "Error translating text";
            }
        }

        private async Task<List<TranslatedSubtitle>> TranslateSubtitlesDeeplPro(List<Subtitle> subtitles)
        {
            List<TranslatedSubtitle> translatedSubtitles = new List<TranslatedSubtitle>();

            foreach (var subtitle in subtitles)
            {
                string translation = await TranslateTextDeeplPro(subtitle.Text);

                translatedSubtitles.Add(new TranslatedSubtitle { Start = subtitle.Start, End = subtitle.End, Text = subtitle.Text, TranslatedText = translation });
            }
            return translatedSubtitles;
        }

        #endregion DeepL Pro Methods

        #region Google Methods

        private string GetGoogleLanguageCode(string language)
        {
            switch (language.ToLower())
            {
                case "?":
                    return "";
                case "afrikaans":
                    return "af";
                case "albanian":
                    return "sq";
                case "amharic":
                    return "am";
                case "arabic":
                    return "ar";
                case "armenian":
                    return "hy";
                case "azerbaijani":
                    return "az";
                case "basque":
                    return "eu";
                case "belarusian":
                    return "be";
                case "bengali":
                    return "bn";
                case "bosnian":
                    return "bs";
                case "bulgarian":
                    return "bg";
                case "catalan":
                    return "ca";
                case "cebuano":
                    return "ceb";
                case "chinese (simplified)":
                    return "zh-CN";
                case "chinese (traditional)":
                    return "zh-TW";
                case "corsican":
                    return "co";
                case "croatian":
                    return "hr";
                case "czech":
                    return "cs";
                case "danish":
                    return "da";
                case "dutch":
                    return "nl";
                case "english":
                    return "en";
                case "esperanto":
                    return "eo";
                case "estonian":
                    return "et";
                case "finnish":
                    return "fi";
                case "french":
                    return "fr";
                case "frisian":
                    return "fy";
                case "galician":
                    return "gl";
                case "georgian":
                    return "ka";
                case "german":
                    return "de";
                case "greek":
                    return "el";
                case "gujarati":
                    return "gu";
                case "haitian creole":
                    return "ht";
                case "hausa":
                    return "ha";
                case "hawaiian":
                    return "haw";
                case "hebrew":
                    return "he";
                case "hindi":
                    return "hi";
                case "hmong":
                    return "hmn";
                case "hungarian":
                    return "hu";
                case "icelandic":
                    return "is";
                case "igbo":
                    return "ig";
                case "indonesian":
                    return "id";
                case "irish":
                    return "ga";
                case "italian":
                    return "it";
                case "japanese":
                    return "ja";
                case "javanese":
                    return "jw";
                case "kannada":
                    return "kn";
                case "kazakh":
                    return "kk";
                case "khmer":
                    return "km";
                case "kinyarwanda":
                    return "rw";
                case "korean":
                    return "ko";
                case "kurdish (kurmanji)":
                    return "ku";
                case "kyrgyz":
                    return "ky";
                case "lao":
                    return "lo";
                case "latin":
                    return "la";
                case "latvian":
                    return "lv";
                case "lithuanian":
                    return "lt";
                case "luxembourgish":
                    return "lb";
                case "macedonian":
                    return "mk";
                case "malagasy":
                    return "mg";
                case "malay":
                    return "ms";
                case "malayalam":
                    return "ml";
                case "maltese":
                    return "mt";
                case "maori":
                    return "mi";
                case "marathi":
                    return "mr";
                case "mongolian":
                    return "mn";
                case "myanmar (burmese)":
                    return "my";
                case "nepali":
                    return "ne";
                case "norwegian":
                    return "no";
                case "nyanja (chichewa)":
                    return "ny";
                case "odia (oriya)":
                    return "or";
                case "pashto":
                    return "ps";
                case "persian":
                    return "fa";
                case "polish":
                    return "pl";
                case "portuguese":
                    return "pt";
                case "punjabi":
                    return "pa";
                case "romanian":
                    return "ro";
                case "russian":
                    return "ru";
                case "samoan":
                    return "sm";
                case "scots gaelic":
                    return "gd";
                case "serbian":
                    return "sr";
                case "sesotho":
                    return "st";
                case "shona":
                    return "sn";
                case "sindhi":
                    return "sd";
                case "sinhala":
                    return "si";
                case "slovak":
                    return "sk";
                case "slovenian":
                    return "sl";
                case "somali":
                    return "so";
                case "spanish":
                    return "es";
                case "sundanese":
                    return "su";
                case "swahili":
                    return "sw";
                case "swedish":
                    return "sv";
                case "tagalog (filipino)":
                    return "tl";
                case "tajik":
                    return "tg";
                case "tamil":
                    return "ta";
                case "tatar":
                    return "tt";
                case "telugu":
                    return "te";
                case "thai":
                    return "th";
                case "turkish":
                    return "tr";
                case "turkmen":
                    return "tk";
                case "ukrainian":
                    return "uk";
                case "urdu":
                    return "ur";
                case "uyghur":
                    return "ug";
                case "uzbek":
                    return "uz";
                case "vietnamese":
                    return "vi";
                case "welsh":
                    return "cy";
                case "xhosa":
                    return "xh";
                case "yiddish":
                    return "yi";
                case "yoruba":
                    return "yo";
                case "zulu":
                    return "zu";
                default:
                    return "en";
            }
        }

        private async Task<string> TranslateTextGoogle(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return sentence;
            }

            try
            {
                string inputLanguage = GetGoogleLanguageCode(comboBoxGoogleInput.Text);
                string outputLanguage = GetGoogleLanguageCode(comboBoxGoogleOutput.Text);

                string apiKey = Properties.Settings.Default.apiKeyGoogle;
                string requestUri = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";

                var requestBody = new
                {
                    q = sentence, 
                    source = inputLanguage,
                    target = outputLanguage,
                    format = "text"
                };

                using (HttpClient client = new HttpClient())
                {
                    string jsonContent = JsonConvert.SerializeObject(requestBody);
                    HttpContent httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(requestUri, httpContent);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    string translatedText = json["data"]["translations"][0]["translatedText"].ToString();

                    return translatedText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Translation error: " + ex.Message);
                return "Error translating text";
            }
        }

        private async Task<List<TranslatedSubtitle>> TranslateSubtitlesGoogle(List<Subtitle> subtitles)
        {
            List<TranslatedSubtitle> translatedSubtitles = new List<TranslatedSubtitle>();

            foreach (var subtitle in subtitles)
            {
                string translation = await TranslateTextGoogle(subtitle.Text);

                translatedSubtitles.Add(new TranslatedSubtitle { Start = subtitle.Start, End = subtitle.End, Text = subtitle.Text, TranslatedText = translation });
            }
            return translatedSubtitles;
        }

        #endregion Google Methods

        #endregion API Methods

        #region VTT Methods

        public List<Subtitle> VttToListOfSubtitle(string filePath)
        {
            var subtitles = new List<Subtitle>();
            string[] lines = File.ReadAllLines(filePath);

            Subtitle currentSubtitle = null;
            var timeSpanRegex = new Regex(@"(\d{2}:\d{2}:\d{2}\.\d{3})\s-->\s(\d{2}:\d{2}:\d{2}\.\d{3})");

            foreach (var line in lines)
            {
                var match = timeSpanRegex.Match(line);
                if (match.Success)
                {
                    currentSubtitle = new Subtitle
                    {
                        Start = TimeSpan.Parse(match.Groups[1].Value),
                        End = TimeSpan.Parse(match.Groups[2].Value)
                    };
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (currentSubtitle != null)
                    {
                        currentSubtitle.Text += (string.IsNullOrWhiteSpace(currentSubtitle.Text) ? "" : "\n") + line;
                    }
                }
                else if (currentSubtitle != null)
                {
                    subtitles.Add(currentSubtitle);
                    currentSubtitle = null;
                }
            }

            if (currentSubtitle != null)
            {
                subtitles.Add(currentSubtitle);
            }

            return subtitles;
        }

        private void CreateVttFile(List<TranslatedSubtitle> subtitles, string filePath)
        {
            StringBuilder vttContent = new StringBuilder();
            vttContent.AppendLine("WEBVTT\n");

            foreach (var subtitle in subtitles)
            {
                // Format the Start and End times in "hh:mm:ss.mmm" format
                string start = $"{subtitle.Start:hh\\:mm\\:ss\\.fff}";
                string end = $"{subtitle.End:hh\\:mm\\:ss\\.fff}";

                // Append the timestamp
                vttContent.AppendLine($"{start} --> {end}");

                // Append the text
                vttContent.AppendLine(subtitle.TranslatedText);

                // Add an empty line after each subtitle entry
                vttContent.AppendLine();
            }

            // Write the content to the VTT file
            File.WriteAllText(filePath, vttContent.ToString());
        }

        private void CreateVttFileCombined(List<TranslatedSubtitle> subtitles, string filePath)
        {
            StringBuilder vttContent = new StringBuilder();
            vttContent.AppendLine("WEBVTT\n");

            foreach (var subtitle in subtitles)
            {
                // Format the Start and End times in "hh:mm:ss.mmm" format
                string start = $"{subtitle.Start:hh\\:mm\\:ss\\.fff}";
                string end = $"{subtitle.End:hh\\:mm\\:ss\\.fff}";

                // Append the timestamp
                vttContent.AppendLine($"{start} --> {end}");

                // Append the text
                vttContent.AppendLine(subtitle.Text);
                vttContent.AppendLine($"[{subtitle.TranslatedText}]");

                // Add an empty line after each subtitle entry
                vttContent.AppendLine();
            }

            // Write the content to the VTT file
            File.WriteAllText(filePath, vttContent.ToString());
        }

        #endregion VTT Methods

    }
}
