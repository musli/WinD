/*===================================================
* 类名称: ObservableCollectionExtensions
* 类描述: ObservableCollection 集合的扩展方法类
* 创建人: musli
* 创建时间: 2020/10/30 17:31:24
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WinD.Common.Extensions
{
    /// <summary>
    /// ObservableCollection 集合的扩展方法类
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// 设置集合筛选器，视图会自动刷新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">集合数据源</param>
        /// <param name="filter">筛选器，若要取消筛选，可以设置为null，也可以使用RemoveFilter</param>
        public static void SetFilter<T>(this IList<T> collection, Predicate<T> filter)
        {
            var objectFilter = new Predicate<object>(o => filter((T)o));
            CollectionViewSource.GetDefaultView(collection).Filter = objectFilter;
        }
        /// <summary>
        /// 移除筛选器，相当于SetFilter(null)，视图会自动刷新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void RemoveFilter<T>(this IList<T> collection)
        {
            CollectionViewSource.GetDefaultView(collection).Filter = null;
        }
        /// <summary>
        /// 批量添加数据 ，原有旧数据不清空
        /// </summary>
        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> newItems)
        {
            if (newItems == null || newItems.Count() == 0) return;
            foreach (var item in newItems)
            {
                collection.Add(item);
            }
        }
        /// <summary>
        /// 批量添加数据，且原有旧数据清空
        /// </summary>
        public static void AddNewRange<T>(this IList<T> collection, IEnumerable<T> newItems)
        {
            collection.Clear();
            AddRange(collection, newItems);
        }
        /// <summary>
        /// 转为ObservableCollection集合
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> sourceCollection)
        {
            return new ObservableCollection<T>(sourceCollection);
        }

        /// <summary>
        /// 根据指定条件移除所有匹配的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceCollection"></param>
        /// <param name="match">条件表达式</param>
        /// <returns>移除元素的数量</returns>
        public static int RemoveAll<T>(this ObservableCollection<T> sourceCollection, Predicate<T> match)
        {
            var tempList = sourceCollection.Where(x => match(x)).ToArray();
            foreach (var target in tempList)
                sourceCollection.Remove(target);

            return tempList.Count();
        }
        /// <summary>
        /// 将当前集合填充到已有ObservableCollection集合，原旧数据会清空,效果跟AddNewRange一样
        /// </summary>
        public static void ToObservableCollection<T>(this IEnumerable<T> sourceCollection, ObservableCollection<T> targetCollection)
        {
            targetCollection.AddNewRange(sourceCollection);
        }
        /// <summary>
        /// 取得当前项，需要控件端设置IsSynchronizedWithCurrentItem="True"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T CurrentItem<T>(this IList<T> collection)
        {
            return (T)CollectionViewSource.GetDefaultView(collection).CurrentItem;
        }
        /// <summary>
        /// 移除当前选中项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public static void RemoveCurrentItem<T>(this IList<T> collection)
        {
            T item = collection.CurrentItem();
            if (item != null)
                collection.Remove(item);
        }
        /// <summary>
        /// 选中某一项，通过索引指定，需先判断是否存在，控件端需设置IsSynchronizedWithCurrentItem="True"
        /// </summary>
        /// <param name="index">索引</param>
        public static void SelectIndex<T>(this IList<T> collection, int index)
        {
            CollectionViewSource.GetDefaultView(collection).MoveCurrentToPosition(index);
        }
        /// <summary>
        /// 选中某个指定项,控件端需设置IsSynchronizedWithCurrentItem="True"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void SelectToItem<T>(this IList<T> collection, object item)
        {
            CollectionViewSource.GetDefaultView(collection).MoveCurrentTo(item);
        }
        /// <summary>
        /// 自动处理序号的问题，每次集合发生变化都会刷序号
        /// </summary>
        /// <typeparam name="T">表格的数据源集合泛型参数</typeparam>
        /// <param name="collectionControl">ObservableCollection集合</param>
        /// <param name="numberPropertyName">序号列对应的属性名称</param>
        /// <remarks>通过监听CollectionChange事件,重刷序号</remarks>
        public static void AutoSetOrderCode<T>(this ObservableCollection<T> collection, string numberPropertyName = "RowNumber")
        {
            if (collection == null) return;
            void refreshRowNumber()
            {
                for (var i = 1; i <= collection.Count; i++)
                {
                    var property = typeof(T).GetProperty(numberPropertyName);
                    property.SetValue(collection[i - 1], i);
                }
            }
            collection.CollectionChanged += (s, e) => refreshRowNumber();
        }
        /// <summary>
        /// 对每个成员执行操作
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="collection">集合</param>
        /// <param name="action">操作</param>
        public static void ForEach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
