# NeoRoute.API Unit Tests

These tests cover the exact scenarios required by the assignment:

- User registration and login (success + failure)
- Event management (Organiser can create, Participant is rejected)
- Role-based access (403 when wrong role)
- Event enrolments (Participant can enrol, Organiser cannot)

## How to run the tests

### From Visual Studio
1. Open the solution (or add the test project to your existing solution).
2. Right-click **NeoRoute.API.Tests** → **Run Tests**.
3. All tests should be green.

### From PowerShell / Terminal
```powershell
cd NeoRoute.API.Tests
dotnet test
```

You should see something like:
```
Passed!  - Failed:     0, Passed:    12, Skipped:     0
```

Take a screenshot of the green results — you will need it for the README and the YouTube video.
