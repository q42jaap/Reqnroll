﻿using FluentAssertions.Execution;
using System.Collections.Immutable;

namespace Reqnroll.FeatureSourceGenerator;

public class AttributeDescriptorTests
{
    public static IEnumerable<object[]> StringRepresentationExamples { get; } =
        [
            [ new AttributeDescriptor("Foo", "Bar"), "[Bar.Foo]" ],
            [ new AttributeDescriptor("Foo", "Bar", [ "Fizz" ]), "[Bar.Foo(\"Fizz\")]" ],
            [ new AttributeDescriptor("Foo", "Bar", [ "Fizz", "Buzz" ]), "[Bar.Foo(\"Fizz\", \"Buzz\")]" ],
            [ new AttributeDescriptor("Foo", "Bar", [ 1, 2 ]), "[Bar.Foo(1, 2)]" ],
            [ new AttributeDescriptor("Foo", "Bar", [ ImmutableArray.Create<string>() ]), "[Bar.Foo(new string[] {})]" ],
            [ 
                new AttributeDescriptor("Foo", "Bar", [ ImmutableArray.Create("potato", "pancakes") ]),
                "[Bar.Foo(new string[] {\"potato\", \"pancakes\"})]" 
            ]
        ];

    [Theory]
    [MemberData(nameof(StringRepresentationExamples))]
    public void DescriptorsCanBeRepresntedAsStrings(AttributeDescriptor attribute, string expected)
    {
        attribute.ToString().Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> DescriptorExamples { get; } =
        [
            [ () => new AttributeDescriptor("Foo", "Bar") ],
            [ () => new AttributeDescriptor("Foo", "Bar", [ "Fizz" ]) ],
            [ () => new AttributeDescriptor("Foo", "Bar", [ "Fizz", "Buzz" ]) ],
            [ () => new AttributeDescriptor("Foo", "Bar", [ 1, 2 ]) ],
            [ () => new AttributeDescriptor("Foo", "Bar", [ ImmutableArray.Create<string>() ]) ],
            [ () => new AttributeDescriptor("Foo", "Bar", [ ImmutableArray.Create("potato") ]) ]
        ];

    [Theory]
    [MemberData(nameof(DescriptorExamples))]
    public void DescriptorsAreEqualWhenTypeNamespaceAndArgumentsAreEquivalent(Func<AttributeDescriptor> example)
    {
        var a = example();
        var b = example();

        using var assertions = new AssertionScope();

        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Equals(b).Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData((byte)100)]
    [InlineData('m')]
    [InlineData(100.01d)]
    [InlineData(100.01f)]
    [InlineData(100)]
    [InlineData(1000L)]
    [InlineData((sbyte)100)]
    [InlineData((short)100.01)]
    [InlineData("muffins")]
    [InlineData(100u)]
    [InlineData(1000ul)]
    [InlineData((ushort)100.01)]
    public void DescriptorsCanBeCreatedWithSomeBuiltInTypesAsArguments(object? argument)
    {
        var attribute = new AttributeDescriptor("Foo", "Bar")
            .WithPositionalArguments(argument)
            .WithNamedArguments(new { Property = argument });

        using var assertions = new AssertionScope();

        attribute.PositionalArguments[0].Should().Be(argument);
        attribute.NamedArguments["Property"].Should().Be(argument);
    }

    [Theory]
    [InlineData(AttributeTargets.Assembly)]
    [InlineData(ConsoleKey.Add)]
    public void DescriptorsCanBeCreatedWithEnumsAsArguments(object? argument)
    {
        var attribute = new AttributeDescriptor("Foo", "Bar")
            .WithPositionalArguments(argument)
            .WithNamedArguments(new { Property = argument });

        using var assertions = new AssertionScope();

        attribute.PositionalArguments[0].Should().Be(argument);
        attribute.NamedArguments["Property"].Should().Be(argument);
    }

    [Theory]
    [InlineData(true)]
    [InlineData((byte)100)]
    [InlineData('m')]
    [InlineData(100.01d)]
    [InlineData(100.01f)]
    [InlineData(100)]
    [InlineData(1000L)]
    [InlineData((sbyte)100)]
    [InlineData((short)100.01)]
    [InlineData("muffins")]
    [InlineData(100u)]
    [InlineData(1000ul)]
    [InlineData((ushort)100.01)]
    public void DescriptorsCanBeCreatedWithImmutableArraysOfSomeBuiltInTypesAsArguments<T>(T value)
    {
        var argument = ImmutableArray.Create(value, value);

        var attribute = new AttributeDescriptor("Foo", "Bar")
            .WithPositionalArguments(argument)
            .WithNamedArguments(new { Property = argument });

        using var assertions = new AssertionScope();

        attribute.PositionalArguments[0].Should().Be(argument);
        attribute.NamedArguments["Property"].Should().Be(argument);
    }

    [Theory]
    [InlineData(AttributeTargets.Assembly)]
    [InlineData(ConsoleKey.Add)]
    public void DescriptorsCanBeCreatedWithImmutableArraysOfEnumsAsArguments<T>(T value)
    {
        var argument = ImmutableArray.Create(value, value);

        var attribute = new AttributeDescriptor("Foo", "Bar")
            .WithPositionalArguments(argument)
            .WithNamedArguments(new { Property = argument });

        using var assertions = new AssertionScope();

        attribute.PositionalArguments[0].Should().Be(argument);
        attribute.NamedArguments["Property"].Should().Be(argument);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(AttributeDescriptorTests))]
    public void DescriptorsCanBeCreatedWithTypesAsArguments(Type argument)
    {
        var attribute = new AttributeDescriptor("Foo", "Bar")
            .WithPositionalArguments(argument)
            .WithNamedArguments(new { Property = argument });

        using var assertions = new AssertionScope();

        attribute.PositionalArguments[0].Should().Be(argument);
        attribute.NamedArguments["Property"].Should().Be(argument);
    }

    [Fact]
    public void DescriptorsCannotBeCreatedWithArraysAsArguments()
    {
        var argument = Array.Empty<string>();

        var attribute = new AttributeDescriptor("Foo", "Bar");

        attribute
            .Invoking(attribute => attribute.WithPositionalArguments([ argument ]))
            .Should().Throw<ArgumentException>();

        attribute
            .Invoking(attribute => attribute.WithNamedArguments(new { Property = argument }))
            .Should().Throw<ArgumentException>();
    }
}
