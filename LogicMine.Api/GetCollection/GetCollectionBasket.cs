﻿/*
MIT License

Copyright(c) 2018
Antonio Di Nucci

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;

namespace LogicMine.Api.GetCollection
{
  /// <summary>
  /// A basket for "Get Collection" operations
  /// </summary>
  /// <typeparam name="T">The type which the basket operates on</typeparam>
  public class GetCollectionBasket<T> : Basket<IGetCollectionRequest<T>, T[]>, IGetCollectionBasket<T>
  {
    /// <inheritdoc />
    public override Type DataType => typeof(T);

    /// <summary>
    /// Construct a new GetCollectionBasket
    /// </summary>
    /// <param name="request">The request</param>
    public GetCollectionBasket(IGetCollectionRequest<T> request) : base(request)
    {
    }

    /// <summary>
    /// Construct a new GetCollectionBasket
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="parent">The basket which is the parent of the current one</param>
    public GetCollectionBasket(IGetCollectionRequest<T> request, IBasket parent) : base(request, parent)
    {
    }
  }
}
