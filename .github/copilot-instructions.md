# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Unit Testing Guidelines
- For unit tests, use lowercase comments for arrange/act/assert sections (// arrange, // act, // assert) and always separate the assert section from act, unless asserting an exception.
- For unit tests on record types, use DocumentDataTests as a template: include Constructor test (sets properties), single With test (changes all properties, verifies new instance with AreNotSame), and Equality tests (same values are equal, different values are not equal). Do not generate additional tests for base record functionality. Only add tests for custom methods on the record type.