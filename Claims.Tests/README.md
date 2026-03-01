# Claims.Tests Structure

Domain-first layout with test type separation:

- `Claims/ControllerTests`: HTTP/controller tests for claims endpoints.
- `Claims/ServiceTests`: business-rule unit tests for claim services.
- `Claims/TestDoubles`: claim-specific fakes used by claim tests.
- `Covers/ControllerTests`: HTTP/controller tests for covers endpoints.
- `Covers/ServiceTests`: business-rule and premium-calculation tests for covers.
- `Covers/TestDoubles`: cover-specific fakes used by cover tests.

Guidelines:

1. Keep tests near the domain they validate.
2. Keep controller tests focused on route/status/contract behavior.
3. Keep service tests focused on business rules and edge cases.
4. Duplicate a tiny fake per domain when it keeps dependencies simpler.