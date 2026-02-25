# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction
- When completing coding tasks, update the corresponding task status in *-tasks.md files located in the plans folder.
- Aim to keep code coverage close to 100% block coverage by adding unit tests to cover all possible conditions and branches.

## Code Style
- Use specific formatting rules
- Follow naming conventions
- Keep code lines to 120 characters or less. If parameter or argument lists fit within 120 characters, keep them on one line. If they exceed 120 characters, wrap them with each parameter/argument on its own line.
- Do not use regions (#region / #endregion) in generated code - it's a code smell.
- Do not add function-level comments (XML doc comments) for code in this solution. Keep code clean without documentation comments on methods unless specifically requested.

## Unit Testing Guidelines
- For unit tests, use lowercase comments for arrange/act/assert sections (// arrange, // act, // assert) and always separate the assert section from act, unless asserting an exception.
- For unit tests on record types, use DocumentDataTests as a template: include Constructor test (sets properties), single With test (changes all properties, verifies new instance with AreNotSame), and Equality tests (same values are equal, different values are not equal). Do not generate additional tests for base record functionality. Only add tests for custom methods on the record type.
- Use MSTest 4 constructs in unit tests. Use Assert.ThrowsExactly<T> instead of Assert.ThrowsException<T> for exception assertions.
- For unit tests, use Assert.HasCount(X, collection) instead of Assert.AreEqual(X, collection.Count) when checking list and array counts.
- For unit test projects, create separate files for exception handling tests (e.g., LineEditingServiceTests.cs and LineEditingServiceTests.Exceptions.cs) if the class has more than 12 normal test methods.