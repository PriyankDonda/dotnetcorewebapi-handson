# Contributing to Handson API

Thank you for your interest in contributing to Handson API! This document provides guidelines and instructions for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Documentation](#documentation)
- [Questions and Discussions](#questions-and-discussions)

## Code of Conduct

By participating in this project, you agree to abide by our Code of Conduct:

- Be respectful and inclusive
- Provide constructive feedback
- Focus on what is best for the community
- Show empathy towards other community members

## Getting Started

1. **Fork the Repository**
   - Click the "Fork" button in the top-right corner of the repository page
   - This creates a copy of the repository in your GitHub account

2. **Clone Your Fork**
   ```bash
   git clone https://github.com/your-username/dotnetcorewebapi-handson.git
   cd dotnetcorewebapi-handson
   ```

3. **Set Up the Development Environment**
   - Install .NET 8.0 SDK
   - Install PostgreSQL 15 or later
   - Install Redis Server
   - Copy `appsettings.Example.json` to `appsettings.json` and configure it

4. **Add the Original Repository as a Remote**
   ```bash
   git remote add upstream https://github.com/PriyankDonda/dotnetcorewebapi-handson.git
   ```

## Development Workflow

1. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   # or
   git checkout -b fix/your-fix-name
   ```

2. **Make Your Changes**
   - Write clean, maintainable code
   - Follow the coding standards
   - Add tests for new functionality
   - Update documentation as needed

3. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "Description of your changes"
   ```

4. **Push to Your Fork**
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**
   - Go to your fork on GitHub
   - Click "New Pull Request"
   - Select the appropriate branches
   - Fill out the pull request template

## Pull Request Process

1. **Before Submitting**
   - Ensure your code follows the coding standards
   - Run all tests
   - Update documentation
   - Rebase your branch on the latest main branch

2. **Pull Request Template**
   - Provide a clear description of the changes
   - Reference any related issues
   - List any breaking changes
   - Include screenshots if applicable

3. **Review Process**
   - Address feedback from reviewers
   - Make requested changes
   - Keep the PR focused on a single change

## Coding Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Write self-documenting code
- Add comments for complex logic
- Keep methods small and focused
- Use async/await for asynchronous operations
- Handle exceptions appropriately
- Follow SOLID principles

## Testing

- Write unit tests for new functionality
- Ensure all tests pass before submitting a PR
- Aim for good test coverage
- Use meaningful test names
- Follow the Arrange-Act-Assert pattern

## Documentation

- Update README.md if needed
- Document new features and APIs
- Add XML comments to public methods
- Keep documentation up-to-date

## Questions and Discussions

- Open an issue for questions or discussions
- Use the issue templates
- Be specific about your question
- Provide relevant context

Thank you for contributing to Handson API! 