using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ColumnalCipher.ViewModels
{
    public class ColumnalViewModel : INotifyPropertyChanged
    {
        public Models.Columnal Model { get; set; }
        
        public ICommand DoEncrypt { get; set; }
        public ICommand DoDecrypt { get; set; }

        public string TextMatrix
        {
            get
            {
                return Model.Matrix;
            }
            set
            {
                Model.Matrix = value;
                NotifyPropertyChanged(nameof(TextMatrix));
            }
        }

        public string OutputText
        {
            get
            {
                return Model.OutputText;
            }
            set
            {
                Model.OutputText = value;
                NotifyPropertyChanged(nameof(OutputText));
            }
        }

        public ColumnalViewModel()
        {
            Model = new Models.Columnal();
            DoEncrypt = new DelegateCommand(Encrypt);
            DoDecrypt = new DelegateCommand(Decrypt);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Encrypt()
        {
            try
            {
                var length = Model.Key.Length;
                int height = Model.InputText.Length / length;
                height = ((double)Model.InputText.Length / length) % 2==0 ? height : height + 1;
                var Matrix = new char[height, length];
                var n = 0;
                bool endWord = false;

                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        if (!endWord) Matrix[i, j] = Model.InputText[n];
                        else Matrix[i, j] = '-';

                        n++;
                        if (n > Model.InputText.Length - 1) endWord = true;
                    }
                }

                var orderedKey = Model.Key.ToList();
                orderedKey.Sort();

                var keyIndices = Model.Key.ToList().Select(c =>
                {
                    var ii = orderedKey.IndexOf(c);
                    orderedKey[ii] = '—';
                    return ii;
                }).ToList();

                var tempMatrix = Matrix.Clone() as char[,];
                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        Matrix[i, j] = tempMatrix[i, keyIndices[j]];
                    }
                }

                string result = "";
                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        result += Matrix[i, j].ToString() + " ";
                    }
                    result += '\n';
                }

                TextMatrix = result;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
            }
        }

        public void Decrypt()
        {
            try
            {
                //todo: finis this shitt
                var length = Model.Key.Length;
                int height = Model.InputText.Length / length;
                height = ((double)Model.InputText.Length / length) % 2 == 0 ? height : height + 1;
                var Matrix = new char[height, length];
                var MatrixRows = Model.Matrix.Replace(" ", "").Split('\n');

                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < length; j++) Matrix[i, j] = MatrixRows[i][j];
                }

                var orderedKey = Model.Key.ToList();
                orderedKey.Sort();

                var keyIndices = Model.Key.ToList().Select(c =>
                {
                    var ii = orderedKey.IndexOf(c);
                    orderedKey[ii] = '—';
                    return ii;
                }).ToList();

                var tempMatrix = Matrix.Clone() as char[,];
                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < length; j++)
                    {
                        Matrix[i, j] = tempMatrix[i, keyIndices[j]];
                    }
                }

                var solution = string.Empty;
                for (var i = 0; i < height; i++)
                    for (var j = 0; j < length; j++) solution += Matrix[i, j].ToString();

                OutputText = solution.Replace("-","");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}