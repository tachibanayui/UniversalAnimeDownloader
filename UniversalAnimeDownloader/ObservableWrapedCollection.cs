﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// An ultility class support break a whole <see cref="ObservableCollection{T}"/> to the 2 dimensional one that fit to the container
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    public class ObservableWrapedCollection<T>
    {
        #region Private Fields
        private List<T> _DefaultItems;
        private int _ItemPerRow;
        private int _CurrentRowNumber;
        private int _LastRowCount;
        private object _LockObject;
        #endregion


        #region Indexer
        public T this[int index]
        {
            get => _DefaultItems[index];
            set
            {
                _DefaultItems[index] = value;
                UpdateWrapView(value, index);
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The collection created for databinding
        /// </summary>
        public ObservableCollection<ObservableCollection<T>> Data { get; set; } = new ObservableCollection<ObservableCollection<T>>();
        /// <summary>
        /// The area which the items will be populated
        /// </summary>
        public double UsableContainerWidth { get; protected set; }


        /// <summary>
        /// The width of the ItemsControl (ListView, ListBox,...) which contains the data items
        /// </summary>
        private double _ContainerWidth;
        public double ContainerWidth
        {
            get { return _ContainerWidth; }
            set
            {
                _ContainerWidth = value;
                var widthDelta = UsableContainerWidth - value;
                if (widthDelta < 0 || widthDelta >= 300)
                    AddRangeAsync(ResetWrapCollection());
            }
        }

        /// <summary>
        /// The width of the items (usually Datatemplate Width) inside the ItemContainer
        /// </summary>
        private double _ItemsWidth;
        public double ItemsWidth
        {
            get { return _ItemsWidth; }
            set
            {
                _ItemsWidth = value;
                var shouldBeUsedWidth = value * _ItemPerRow;
                if (shouldBeUsedWidth > ContainerWidth || shouldBeUsedWidth <= ContainerWidth + value)
                    AddRangeAsync(ResetWrapCollection());
            }
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Initialize a new instance of <see cref="ObservableWrapedCollection{T}"/> class with specified Container and Items width
        /// </summary>
        /// <param name="containerWidth">The width of the ItemsControl (ListView, ListBox,...) which contains the data items</param>
        /// <param name="itemsWidth">The width of the items (usually Datatemplate Width) inside the ItemContainer</param>
        public ObservableWrapedCollection(double containerWidth, double itemsWidth)
        {
            _LockObject = new object();
            BindingOperations.EnableCollectionSynchronization(Data, _LockObject);
            _ContainerWidth = containerWidth;
            _ItemsWidth = itemsWidth;
            ResetWrapCollection();
        }

        /// <summary>
        /// Add an object to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The value can be null for reference type</param>
        public void Add(T item)
        {
            if (_LastRowCount < _ItemPerRow)
            {
                lock (_LockObject)
                {
                    Data[_CurrentRowNumber].Add(item);
                }

                _LastRowCount++;
            }
            else
            {
                var newCol = new ObservableCollection<T>();
                BindingOperations.EnableCollectionSynchronization(newCol, _LockObject);
                lock (_LockObject)
                {
                    Data.Add(newCol);
                    _CurrentRowNumber++;
                    Data[_CurrentRowNumber].Add(item);
                }
                _LastRowCount = 1;
            }

            _DefaultItems.Add(item);
        }

        /// <summary>
        /// Add an object to the end of the 2 dimensional <see cref="ObservableCollection{T}"/> as an asynchronous operation.
        /// </summary>
        /// <param name="item">The object to be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The value can be null for reference type</param>
        public async void AddAsync(T item) => await Task.Run(() => Add(item));

        /// <summary>
        /// Add an object to the end of the 2 dimensional <see cref="ObservableCollection{T}"/> as an asynchronous operation.
        /// </summary>
        /// <param name="item">The object to be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The value can be null for reference type</param>
        public async Task AddAsyncTask(T item) => await Task.Run(() => Add(item));

        /// <summary>
        /// Add elements to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The collection itself cannot be null but it can contain elements that are null, if type T is a reference type.</param>
        public void AddRange(IList<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        /// <summary>
        /// Add elements to the end of the 2 dimensional <see cref="ObservableCollection{T}"/> as an asynchronous operation..
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The collection itself cannot be null but it can contain elements that are null, if type T is a reference type.</param>
        public async void AddRangeAsync(IList<T> items) => await Task.Run(() => AddRange(items));

        /// <summary>
        /// Add elements to the end of the 2 dimensional <see cref="ObservableCollection{T}"/> as an asynchronous operation..
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of the 2 dimensional <see cref="ObservableCollection{T}"/>. The collection itself cannot be null but it can contain elements that are null, if type T is a reference type.</param>
        public async Task AddRangeAsyncTask(IList<T> items) => await Task.Run(() => AddRange(items));

        /// <summary>
        /// Removes all elements from the 2 dimensional <see cref="ObservableCollection{T}"/>
        /// </summary>
        public void Clear() => ResetWrapCollection();
        #endregion

        #region PrivateMethod
        /// <summary>
        /// Use for reset the <see cref="Data"/> and <see cref="_DefaultItems"/>. This method used when <see cref="ObservableWrapedCollection{T}"/> is created or cleared
        /// </summary>
        /// <returns>The elements belong to <see cref="_DefaultItems"/> before cleared.</returns>
        private List<T> ResetWrapCollection()
        {
            if (_DefaultItems == null)
                _DefaultItems = new List<T>();

            _ItemPerRow = (int)Math.Floor(ContainerWidth / ItemsWidth);
            UsableContainerWidth = ContainerWidth * ItemsWidth;
            _CurrentRowNumber = 0;
            _LastRowCount = 0;
            var res = new T[_DefaultItems.Count];
            _DefaultItems.CopyTo(res);
            _DefaultItems.Clear();
            Data.Clear();
            var newCol = new ObservableCollection<T>();
            BindingOperations.EnableCollectionSynchronization(newCol, _LockObject);
            Data.Add(newCol);
            return res.ToList();
        }

        /// <summary>
        /// Find and assign value to the 2 dimensional <see cref="ObservableCollection{T}"/>. This method used when user set an item through this class's indexer
        /// </summary>
        /// <param name="value">The new value to be assigned</param>
        /// <param name="index">The index of the element will be assigned</param>
        private void UpdateWrapView(T value, int index)
        {
            lock (_LockObject)
            {
                Data[(int)Math.Ceiling(index / (double)_ItemPerRow)][index % _ItemPerRow] = value;
            }
        }
        #endregion
    }
}
