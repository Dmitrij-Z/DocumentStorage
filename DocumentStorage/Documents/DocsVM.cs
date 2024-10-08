using DocumentStorage.BaseClasses;
using DocumentStorage.DB;
using DocumentStorage.HelpClasses;
using DocumentStorage.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DocumentStorage.Documents
{
    class DocsVM : BaseInpc
    {
        #region Подключение классов: расчета и взаимодействия с БД/переменные

        Calculate calc = new Calculate();
        DBRequests dBRequests;

        string tempDirr = Path.Combine(Environment.CurrentDirectory, "TempFiles");

        #endregion

        #region Конструктор

        public DocsVM()
        {
            SearchConditions = new List<SearchCondition>()
            {
                new SearchCondition(){ Id=0, TextValue="Слова в строгом порядке" },
                new SearchCondition(){ Id=1, TextValue="Слова в любом порядке" },
                new SearchCondition(){ Id=2, TextValue="Содержит хотя бы одно слово" }
            };
            dBRequests = new DBRequests();
            CurrError = dBRequests.DbChecking();
            if (!string.IsNullOrEmpty(CurrError))
            {
                return;
            }
            string err = string.Empty;
            Docs = new ObservableCollection<Doc>(dBRequests.GetAllDocs(out err));
            CurrError = err;
            DocsView = CollectionViewSource.GetDefaultView(Docs);
            DocsView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            DocsView.Filter = DocsFilter;
            SelectedCondition = SearchConditions[0];
        }

        #endregion

        #region Переменные/коллекция

        public ObservableCollection<Doc> Docs { get;}
        
        private Doc _selectedDoc;
        public Doc SelectedDoc
        {
            get => _selectedDoc;
            set => Set(ref _selectedDoc, value);
        }

        private string _currError = string.Empty;
        public string CurrError
        {
            get => _currError;
            set => Set(ref _currError, value);
        }
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => Set(ref _searchText, value);
        }
        private bool _editMode = false;
        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                Set(ref _editMode, value);
                RaisePropertyChanged("IsCurrDocSampleVisible");
            }
        }

        private string _focusedTextboxName;
        public string FocusedTextboxName
        {
            get => _focusedTextboxName;
            set => Set(ref _focusedTextboxName, value);
        }

        #region Переменные текущего документа

        private string _currentTitle;
        public string CurrentTitle
        {
            get => _currentTitle;
            set => Set(ref _currentTitle, value ?? string.Empty);
        }
        private string _currentFileName;
        public string CurrentFileName
        {
            get => _currentFileName;
            set => Set(ref _currentFileName, value ?? string.Empty);
        }
        private string _currentComment;
        public string CurrentComment
        {
            get => _currentComment;
            set => Set(ref _currentComment, value ?? string.Empty);
        }
        private byte[] _currentDocData;
        public byte[] CurrentDocData
        {
            get => _currentDocData;
            set => Set(ref _currentDocData, value);
        }
        private byte[] _currentSampleDocData;
        public byte[] CurrentSampleDocData
        {
            get => _currentSampleDocData;
            set => Set(ref _currentSampleDocData, value);
        }
        private string _currDocCreatedOn;
        public string CurrDocCreatedOn
        {
            get => _currDocCreatedOn;
            set => Set(ref _currDocCreatedOn, value);
        }
        private string _currDocUpdatedOn;
        public string CurrDocUpdatedOn
        {
            get => _currDocUpdatedOn;
            set => Set(ref _currDocUpdatedOn, value);
        }

        #endregion

        private bool _isCurrDocChanged = false;
        public bool IsCurrDocChanged
        {
            get => _isCurrDocChanged;
            set => Set(ref _isCurrDocChanged, value);
        }

        #endregion

        #region Фильтр коллекции

        public ICollectionView DocsView;

        public List<SearchCondition> SearchConditions { get; }

        private SearchCondition _selectedCondition;
        public SearchCondition SelectedCondition
        {
            get { return _selectedCondition; }
            set
            {
                _selectedCondition = value;
                RaisePropertyChanged();
                DocsView.Refresh();
            }
        }

        private bool _caseSensitive;
        public bool CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                _caseSensitive = value;
                RaisePropertyChanged();
                DocsView.Refresh();
            }
        }

        private bool DocsFilter(object value)
        {
            Doc doc = value as Doc;
            if (doc != null)
            {
                return IsFound(doc, SearchText, CaseSensitive);
            }
            return false;
        }

        private bool IsFound(Doc doc, string searchTxt, bool caseSensitive)
        {
            try
            {
                searchTxt = searchTxt.Trim();
                if (SelectedCondition != null)
                {
                    List<string> lst = new List<string>();
                    lst.Add(doc.Title);
                    lst.Add(doc.FileName);
                    lst.Add(doc.Comment);
                    switch (SelectedCondition.Id)
                    {
                        case 0:
                            return calc.ContainsAllInStrictOrder(doc.Title, searchTxt, caseSensitive) ||
                                   calc.ContainsAllInStrictOrder(doc.FileName, searchTxt, caseSensitive) ||
                                   calc.ContainsAllInStrictOrder(doc.Comment, searchTxt, caseSensitive);
                        case 1:
                            return calc.ContainsAllInAnyOrder(string.Join("\r\n", lst.ToArray()), searchTxt, caseSensitive);
                        case 2:
                            return calc.ContainsAtLeastOne(string.Join("\r\n", lst.ToArray()), searchTxt, caseSensitive);
                    }
                }
            }
            catch (Exception ex)
            {
                CurrError = ex.Message;
                return false;
            }
            return false;
        }

        #endregion

        #region Команды

        #region Команда фильтрации

        private RelayCommand _filterChangedCommand;
        public RelayCommand FilterChangedCommand => _filterChangedCommand ?? (_filterChangedCommand = new RelayCommand(FilterChanged));
        private void FilterChanged(object parameter)
        {
            SearchText = parameter as string;
            DocsView.Refresh();
        }
        #endregion

        #region Команды редактора: добавление/изменение/удаление/сброс

        private RelayCommand _addDocCommand;
        public RelayCommand AddDocCommand => _addDocCommand ?? (_addDocCommand = new RelayCommand(AddDoc));
        private void AddDoc()
        {
            DateTime addedDate = DateTime.Now;
            string err = string.Empty;
            CurrError = err;
            Doc doc = new Doc();
            doc.Title = CurrentTitle;
            doc.FileName = CurrentFileName;
            doc.Comment = CurrentComment;
            doc.DocData = CurrentDocData;
            doc.DocSampleData = CurrentSampleDocData;
            doc.CreatedOn = addedDate;
            doc.UpdatedOn = addedDate;
            doc.State = 2;
            doc.Id = dBRequests.InsertDocToDb(doc, out err);
            if (doc.Id >= 0)
            {
                Docs.Add(doc);
                DocsView.MoveCurrentTo(doc);
                SelectedDoc = doc;
                IsCurrDocChanged = false;
            }
            else
            {
                CurrError = err;
            }
        }

        private RelayCommand _changeDocCommand;
        public RelayCommand ChangeDocCommand => _changeDocCommand ?? (_changeDocCommand = new RelayCommand(ChangeDoc));
        private void ChangeDoc()
        {
            Doc doc = new Doc();
            doc.Id = SelectedDoc.Id;
            doc.Title = CurrentTitle;
            doc.FileName = CurrentFileName;
            doc.Comment = CurrentComment;
            doc.DocData = CurrentDocData;
            doc.DocSampleData = CurrentSampleDocData;
            doc.UpdatedOn = DateTime.Now;
            UpdateDoc(doc);
        }

        private RelayCommand _deleteDocCommand;
        public RelayCommand DeleteDocCommand => _deleteDocCommand ?? (_deleteDocCommand = new RelayCommand(DeleteDoc));
        private void DeleteDoc()
        {
            string err = string.Empty;
            CurrError = err;
            MessageBoxResult result = MessageBox.Show(App.Current.MainWindow, "Документ будет безвозвратно удален!\r\nПодтвердить удаление?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                if (dBRequests.DeleteDoc(SelectedDoc.Id, out err))
                {
                    Docs.Remove(SelectedDoc);
                }
                else
                {
                    CurrError = err;
                }
            }
        }

        private RelayCommand _clearDocCommand;
        public RelayCommand ClearDocCommand => _clearDocCommand ?? (_clearDocCommand = new RelayCommand(ClearDoc));
        private void ClearDoc()
        {
            if (SelectedDoc == null)
            {
                ClearDocParameters();
            }
            else
            {
                SelectedDoc = null;
            }
        }

        private RelayCommand _closeErrorFormCommand;
        public RelayCommand CloseErrorFormCommand => _closeErrorFormCommand ?? (_closeErrorFormCommand = new RelayCommand(CloseErrorForm));
        private void CloseErrorForm()
        {
            CurrError = string.Empty;
        }

        private RelayCommand _setFocusOnTextboxCommand;
        public RelayCommand SetFocusOnTextboxCommand => _setFocusOnTextboxCommand ?? (_setFocusOnTextboxCommand = new RelayCommand(SetFocusOnTextbox));
        private void SetFocusOnTextbox(object parameter)
        {
            string tbxName = parameter as string;
            if (!string.IsNullOrEmpty(tbxName))
            {
                FocusedTextboxName = tbxName;
                FocusedTextboxName = string.Empty;
            }
        }
        
        #endregion

        #region Команды текущего документа (файла)

        private RelayCommand _openDocumCommand;
        public RelayCommand OpenDocumCommand => _openDocumCommand ?? (_openDocumCommand = new RelayCommand(OpenDocum));
        private void OpenDocum(object parameter)
        {
            try
            {
                string prm = parameter as string;
                string extension;
                byte[] docdata;

                switch (prm)
                {
                    case "DocFile":
                        extension = Path.GetExtension(CurrentFileName);
                        docdata = CurrentDocData;
                        break;
                    case "DocSampleFile":
                        extension = Path.GetExtension(CurrentFileName);
                        docdata = CurrentSampleDocData;
                        break;
                    default: return;
                }
                if (!Directory.Exists(tempDirr))
                {
                    Directory.CreateDirectory(tempDirr);
                }
                string fileName = Path.GetTempFileName() + extension;
                string shortFileName = Path.Combine(tempDirr, Path.GetFileNameWithoutExtension(CurrentFileName) + "_" + fileName.Substring(fileName.LastIndexOf('\\') + 1));

                using (FileStream fs = new FileStream(shortFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    for (int i = 0; i < docdata.Length; i++)
                    {
                        fs.WriteByte(docdata[i]);
                    }
                }
                Process prc = new Process();
                prc.StartInfo.FileName = shortFileName;
                prc.Start();
            }
            catch (Exception ex)
            {
                CurrError = ex.Message;
            }
        }
        
        private RelayCommand _selectDocumCommand;
        public RelayCommand SelectDocumCommand => _selectDocumCommand ?? (_selectDocumCommand = new RelayCommand(SelectDocum));
        private void SelectDocum(object parameter)
        {
            try
            {
                string prm = parameter as string;
                byte[] docdata = null;

                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Title = "Выберите файл для загрузки (максимально допустимый размер 10 МБ)";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    string fname = ofd.FileName;
                    long length = new System.IO.FileInfo(fname).Length;

                    if (length / 1024 > 10240)
                    {
                        MessageBox.Show("Превышен максимально допустимый размер файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (FileStream fs = new FileStream(fname, FileMode.Open))
                    {
                        docdata = new byte[fs.Length];
                        fs.Read(docdata, 0, docdata.Length);
                    }
                    string filename = fname.Substring(fname.LastIndexOf('\\') + 1);
                    switch (prm)
                    {
                        case "DocFile":
                            CurrentFileName = filename;
                            CurrentDocData = docdata;
                            break;
                        case "DocSampleFile":
                            if (string.IsNullOrEmpty(CurrentFileName))
                            {
                                CurrentFileName = filename;
                            }
                            CurrentSampleDocData = docdata;
                            break;
                        default: return;
                    }
                }
            }
            catch (Exception ex)
            {
                CurrError = ex.Message;
            }
        }
        
        private RelayCommand _deleteDocumCommand;
        public RelayCommand DeleteDocumCommand => _deleteDocumCommand ?? (_deleteDocumCommand = new RelayCommand(DeleteDocum));
        private void DeleteDocum(object parameter)
        {
            try
            {
                string prm = parameter as string;

                switch (prm)
                {
                    case "DocFile":
                        CurrentDocData = null;
                        break;
                    case "DocSampleFile":
                        CurrentSampleDocData = null;
                        break;
                    default: return;
                }
                if(CurrentDocData == null && CurrentSampleDocData == null)
                {
                    CurrentFileName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                CurrError = ex.Message;
            }
        }
        
        private RelayCommand _undoChangesCommand;
        public RelayCommand UndoChangesCommand => _undoChangesCommand ?? (_undoChangesCommand = new RelayCommand(UndoChanges));
        private void UndoChanges()
        {
            try
            {
                SetCurrentParameters();
            }
            catch (Exception ex)
            {
                CurrError = ex.Message;
            }
        }

        #endregion

        #region Вспомогательные методы

        private void SetCurrentParameters()
        {
            if(SelectedDoc == null)
            {
                ClearDocParameters();
            }
            else
            {
                CurrentTitle = SelectedDoc.Title;
                CurrentFileName = SelectedDoc.FileName;
                CurrentDocData = SelectedDoc.DocData;
                CurrentSampleDocData = SelectedDoc.DocSampleData;
                CurrentComment = SelectedDoc.Comment;
                CurrDocCreatedOn = SelectedDoc.CreatedOn.ToString("dd.MM.yyyy");
                CurrDocUpdatedOn = SelectedDoc.UpdatedOn.ToString("dd.MM.yyyy");
            }
            IsCurrDocChanged = false;
        }

        private void ClearDocParameters()
        {
            CurrentTitle = string.Empty;
            CurrentFileName = string.Empty;
            CurrentDocData = null;
            CurrentSampleDocData = null;
            CurrentComment = string.Empty;
            CurrDocCreatedOn = string.Empty;
            CurrDocUpdatedOn = string.Empty;
        }

        private bool UpdateDoc(Doc doc)
        {
            string err = string.Empty;
            string dTitle = string.Empty;
            CurrError = err;
            try
            {
                if (dBRequests.UpdateDoc(doc, out err))
                {
                    for (int i = 0; i < Docs.Count; i++)
                    {
                        if (Docs[i].Id == doc.Id)
                        {
                            dTitle = doc.Title;
                            Docs[i].Title = doc.Title;
                            Docs[i].FileName = doc.FileName;
                            Docs[i].Comment = doc.Comment;
                            Docs[i].DocData = doc.DocData;
                            Docs[i].DocSampleData = doc.DocSampleData;
                            Docs[i].UpdatedOn = doc.UpdatedOn;
                            Docs[i].State = 1;
                            IsCurrDocChanged = false;
                            DocsView.Refresh();
                            return true;
                        }
                    }
                    throw new Exception(string.Format("Странно, но факт: обновляемый документ с Id '{0}' не был найден в коллекции!", doc.Id));
                }
                else
                {
                    throw new Exception(dTitle + "\r\n" + err);
                }
            }
            catch(Exception ex)
            {
                CurrError = ex.Message;
                return false;
            }
        }

        private bool CompareDocData(object val1, object val2)
        {
            if(val1 == val2)
            {
                return true;
            }
            else if(val1 == null || val2== null)
            {
                return false;
            }
            return ((byte[])val1).SequenceEqual((byte[])val2);

        }

        #endregion

        #endregion

        #region Определение изменений документа

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnPropertyChanged(propertyName, oldValue, newValue);

            if (propertyName == "CurrentTitle" || propertyName == "CurrentFileName" || propertyName == "CurrentComment"
                 || propertyName == "CurrentDocData" || propertyName == "CurrentSampleDocData")
            {
                IsCurrDocChanged = !
                    (SelectedDoc == null ||
                    SelectedDoc.Title == CurrentTitle &&
                    SelectedDoc.FileName == CurrentFileName &&
                    SelectedDoc.Comment == CurrentComment &&
                    CompareDocData(SelectedDoc.DocData, CurrentDocData) &&
                    CompareDocData(SelectedDoc.DocSampleData, CurrentSampleDocData));
            }

            if (propertyName == "SelectedDoc")
            {
                CurrentTitle = SelectedDoc?.Title ?? string.Empty;
                CurrentFileName = SelectedDoc?.FileName ?? string.Empty;
                CurrentComment = SelectedDoc?.Comment ?? string.Empty;
                CurrentDocData = SelectedDoc?.DocData;
                CurrentSampleDocData = SelectedDoc?.DocSampleData;
                CurrDocCreatedOn = SelectedDoc?.CreatedOn.ToString("dd.MM.yyyy HH:mm:ss") ?? string.Empty;
                CurrDocUpdatedOn = SelectedDoc?.UpdatedOn.ToString("dd.MM.yyyy HH:mm:ss") ?? string.Empty;
            }
        }

        #endregion
    }
}
