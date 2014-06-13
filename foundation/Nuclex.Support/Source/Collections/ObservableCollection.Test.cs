#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;

#if UNITTEST

using NUnit.Framework;
using NMock;

namespace Nuclex.Support.Collections {

  /// <summary>Unit Test for the observable collection class</summary>
  [TestFixture]
  public class ObservableCollectionTest {

    #region interface IObservableCollectionSubscriber

    /// <summary>Interface used to test the observable collection</summary>
    public interface IObservableCollectionSubscriber {

      /// <summary>Called when the collection is about to clear its contents</summary>
      /// <param name="sender">Collection that is clearing its contents</param>
      /// <param name="arguments">Not used</param>
      void Clearing(object sender, EventArgs arguments);

      /// <summary>Called when the collection has been cleared of its contents</summary>
      /// <param name="sender">Collection that was cleared of its contents</param>
      /// <param name="arguments">Not used</param>
      void Cleared(object sender, EventArgs arguments);

      /// <summary>Called when an item is added to the collection</summary>
      /// <param name="sender">Collection to which an item is being added</param>
      /// <param name="arguments">Contains the item that is being added</param>
      void ItemAdded(object sender, ItemEventArgs<int> arguments);

      /// <summary>Called when an item is removed from the collection</summary>
      /// <param name="sender">Collection from which an item is being removed</param>
      /// <param name="arguments">Contains the item that is being removed</param>
      void ItemRemoved(object sender, ItemEventArgs<int> arguments);

    }

    #endregion // interface IObservableCollectionSubscriber

    /// <summary>Initialization routine executed before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockery = new MockFactory();

      this.mockedSubscriber = this.mockery.CreateMock<IObservableCollectionSubscriber>();

      this.observedCollection = new ObservableCollection<int>();
      this.observedCollection.Clearing += new EventHandler(
        this.mockedSubscriber.MockObject.Clearing
      );
      this.observedCollection.Cleared += new EventHandler(
        this.mockedSubscriber.MockObject.Cleared
      );
      this.observedCollection.ItemAdded += new EventHandler<ItemEventArgs<int>>(
        this.mockedSubscriber.MockObject.ItemAdded
      );
      this.observedCollection.ItemRemoved += new EventHandler<ItemEventArgs<int>>(
        this.mockedSubscriber.MockObject.ItemRemoved
      );
    }

    /// <summary>Tests whether the Clearing event is fired</summary>
    [Test]
    public void TestClearingEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.Clearing(null, null)).WithAnyArguments();
      this.mockedSubscriber.Expects.One.Method(m => m.Cleared(null, null)).WithAnyArguments();
      this.observedCollection.Clear();

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemAdded event is fired</summary>
    [Test]
    public void TestItemAddedEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.ItemAdded(null, null)).WithAnyArguments();

      this.observedCollection.Add(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemRemoved event is fired</summary>
    [Test]
    public void TestItemRemovedEvent() {
      this.mockedSubscriber.Expects.One.Method(m => m.ItemAdded(null, null)).WithAnyArguments();

      this.observedCollection.Add(123);

      this.mockedSubscriber.Expects.One.Method(m => m.ItemRemoved(null, null)).WithAnyArguments();

      this.observedCollection.Remove(123);

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether items in the collection can be replaced</summary>
    [Test]
    public void TestItemReplacement() {
      this.mockedSubscriber.Expects.Exactly(3).Method(
        m => m.ItemAdded(null, null)
      ).WithAnyArguments();

      this.observedCollection.Add(1);
      this.observedCollection.Add(2);
      this.observedCollection.Add(3);

      this.mockedSubscriber.Expects.One.Method(m => m.ItemRemoved(null, null)).WithAnyArguments();
      this.mockedSubscriber.Expects.One.Method(m => m.ItemAdded(null, null)).WithAnyArguments();

      // Replace the middle item with something else
      this.observedCollection[1] = 4;

      Assert.AreEqual(
        1, this.observedCollection.IndexOf(4)
      );

      this.mockery.VerifyAllExpectationsHaveBeenMet();
    }

    /// <summary>Tests whether the ItemRemoved event is fired</summary>
    [Test]
    public void TestListConstructor() {
      int[] integers = new int[] { 12, 34, 56, 78 };

      ObservableCollection<int> testCollection = new ObservableCollection<int>(integers);

      CollectionAssert.AreEqual(integers, testCollection);
    }

    /// <summary>Mock object factory</summary>
    private MockFactory mockery;
    /// <summary>The mocked observable collection subscriber</summary>
    private Mock<IObservableCollectionSubscriber> mockedSubscriber;
    /// <summary>An observable collection to which a mock will be subscribed</summary>
    private ObservableCollection<int> observedCollection;

  }

} // namespace Nuclex.Support.Collections

#endif // UNITTEST
