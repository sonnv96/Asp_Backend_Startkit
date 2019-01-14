using Nois.Framework.Services;
using Nois.WpfApp.Framework.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Nois.App.Pager.Views
{
    public class PagerViewModel<T> : ViewModel
    {
        public PagerViewModel(Func<int, int, IPagedList<T>> getData, Action<IPagedList<T>> dataChange)
        {
            _getData = getData;
            _dataChange = dataChange;
            PageSizes = new ObservableCollection<PageSizeItem>(
                new List<PageSizeItem> {
                    new PageSizeItem { PageSize = 10, Display = "10" },
                    new PageSizeItem { PageSize = 15, Display = "15" },
                    new PageSizeItem { PageSize = 20, Display = "20" },
                    new PageSizeItem { PageSize = 30, Display = "30" },
                    new PageSizeItem { PageSize = 50, Display = "50" },
                    new PageSizeItem { PageSize = int.MaxValue, Display = "All" }
                });
            _pageSize = PageSizes.FirstOrDefault();
            RaisePropertyChanged("PageSize");
        }

        private Func<int, int, IPagedList<T>> _getData;
        private Action<IPagedList<T>> _dataChange;

        private ObservableCollection<PageItem> _pageItems;
        public ObservableCollection<PageItem> PageItems
        {
            get { return _pageItems; }
            set
            {
                _pageItems = value;
                RaisePropertyChanged("PageItems");
            }
        }

        private ObservableCollection<PageSizeItem> _pageSizes;
        public ObservableCollection<PageSizeItem> PageSizes
        {
            get { return _pageSizes; }
            set
            {
                _pageSizes = value;
                RaisePropertyChanged("PageSizes");
            }
        }

        private PageSizeItem _pageSize;
        public PageSizeItem PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged("PageSize");
                if (_pageSize != null && _pageSize.PageSize != 0)
                    Load();
            }
        }

        public int TotalPages { get; set; }
        public int CurrentIndex { get; set; }
        private int _total;
        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged("Total");
            }
        }

        private int _specialTotal;
        public int SpecialTotal
        {
            get { return _specialTotal; }
            set
            {
                _specialTotal = value;
                RaisePropertyChanged("SpecialTotal");
            }
        }

        public ICommand PageClick
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (obj.ToString() == "Previous")
                        CurrentIndex--;
                    else if (obj.ToString() == "Next")
                        CurrentIndex++;
                    else
                        CurrentIndex = int.Parse(obj.ToString());

                    Load();
                });
            }
        }

        private void UpdatePager()
        {
            TotalPages = (int)Math.Ceiling((decimal)Total / PageSize.PageSize);
            if (TotalPages != 0 && CurrentIndex > TotalPages)
            {
                CurrentIndex = Math.Max(1, TotalPages);
                Load();
                return;
            }

            var pageItems = new List<PageItem>();
            var previous = new PageItem
            {
                Index = "Previous",
                PageIndexChange = PageClick
            };
            pageItems.Add(previous);
            var next = new PageItem
            {
                Index = "Next",
                PageIndexChange = PageClick
            };
            if (CurrentIndex == 1 || TotalPages == 0)
                previous.ItemType = "NoAction";
            if (CurrentIndex == TotalPages || TotalPages == 0)
                next.ItemType = "NoAction";

            pageItems.AddRange(Enumerable.Range(Math.Max(1, CurrentIndex - 3 - (TotalPages - CurrentIndex > 3 ? 0 : 3 - TotalPages + CurrentIndex)), Math.Min(7, TotalPages)).Select(i => new PageItem { Index = i.ToString(), PageIndexChange = PageClick }));

            if (TotalPages > 7 && pageItems[2].Index != "2")
            {
                pageItems[2].Index = "...";
                pageItems[2].ItemType = "NoAction";
                pageItems[1].Index = "1";
            }
            if (TotalPages > 7 && pageItems[6].Index != (TotalPages - 1).ToString())
            {
                pageItems[6].Index = "...";
                pageItems[6].ItemType = "NoAction";
                pageItems[7].Index = TotalPages.ToString();
            }

            var selected = pageItems.FirstOrDefault(p => p.Index == CurrentIndex.ToString());
            if (selected != null)
                selected.ItemType = "IsSelected";

            pageItems.Add(next);
            PageItems = new ObservableCollection<PageItem>(pageItems);
        }

        public void Load()
        {
            var data = _getData(_pageSize.PageSize, CurrentIndex);
            Total = data.TotalCount;
            _dataChange(data);
            UpdatePager();
            
        }
        public void Refresh(int total)
        {
            CurrentIndex = 1;
            Total = total;
            UpdatePager();
        }
    }
    public class PageItem
    {
        public string Index { get; set; }
        public ICommand PageIndexChange { get; set; }
        public string ItemType { get; set; }
    }
    public class PageSizeItem
    {
        public int PageSize { get; set; }
        public string Display { get; set; }
    }
}
