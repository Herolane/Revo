﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Revo.Core.Types;
using Xunit;

namespace Revo.Core.Tests.Types
{
    public class TypeIndexerTests
    {
        private TypeIndexer sut;
        private ITypeExplorer typeExplorer;

        public TypeIndexerTests()
        {
            typeExplorer = Substitute.For<ITypeExplorer>();
            typeExplorer.GetAllTypes().Returns(new[]
            {
                typeof(TestIndexed),
                typeof(TestIndexedMoreVersions),
                typeof(TestIndexedMoreVersionsV2),
                typeof(TestIndexedRenamed),
                typeof(TestNotIndexedAbstract),
                typeof(TestNotIndexedGeneric<>),
                typeof(TestNotIndexedGeneric<>).MakeGenericType(typeof(string)),
                typeof(ITestNotIndexedInterface),
                typeof(NotIndexedUnrelated)
            });

            sut = new TypeIndexer(typeExplorer);
        }

        [Fact]
        public void IndexesDefault()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().Contain(new VersionedType(new VersionedTypeId("TestIndexed", 1), typeof(TestIndexed)));
        }

        [Fact]
        public void IndexesVersionByAttribute()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().Contain(new VersionedType(new VersionedTypeId("TestIndexedMoreVersions", 3), typeof(TestIndexedMoreVersions)));
        }

        [Fact]
        public void IndexesVersionByName()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().Contain(new VersionedType(new VersionedTypeId("TestIndexedMoreVersions", 2), typeof(TestIndexedMoreVersionsV2)));
        }

        [Fact]
        public void IndexedRenamed()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().Contain(new VersionedType(new VersionedTypeId("TestIndexedWithNewName", 42), typeof(TestIndexedRenamed)));
        }

        [Fact]
        public void DoesNotIndexAbstract()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().NotContain(x => x.ClrType == typeof(TestNotIndexedAbstract));
        }

        [Fact]
        public void DoesNotIndexGeneric()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().NotContain(x => x.ClrType == typeof(TestNotIndexedGeneric<>));
            result.Should().NotContain(x => x.ClrType == typeof(TestNotIndexedGeneric<>).MakeGenericType(typeof(string)));
        }

        [Fact]
        public void DoesNotIndexInterface()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().NotContain(x => x.ClrType == typeof(ITestNotIndexedInterface));
        }

        [Fact]
        public void DoesNotIndexUnrelated()
        {
            var result = sut.IndexTypes<ITestIndexed>();
            result.Should().NotContain(x => x.ClrType == typeof(NotIndexedUnrelated));
        }

        public interface ITestIndexed
        {            
        }

        public class TestIndexed : ITestIndexed
        {
        }
        
        [TypeVersion(3)]
        public class TestIndexedMoreVersions : ITestIndexed
        {
        }

        public class TestIndexedMoreVersionsV2 : ITestIndexed
        {
        }

        [TypeVersion("TestIndexedWithNewName", 42)]
        public class TestIndexedRenamed : ITestIndexed
        {
        }
        
        public abstract class TestNotIndexedAbstract : ITestIndexed
        {
        }

        public class TestNotIndexedGeneric<T> : ITestIndexed
        {
        }

        public interface ITestNotIndexedInterface : ITestIndexed
        {
        }

        public class NotIndexedUnrelated
        {
        }
    }
}
