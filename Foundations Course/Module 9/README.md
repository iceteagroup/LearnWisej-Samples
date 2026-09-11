# WisejTrainingApp · Wisej.NET Foundations · Module 9

The deployment review screen from **Module 9 · Configuration, Security & Deployment**, as the walkthrough video
builds it: release information (environment, target, version, reviewer), a required and an optional
checklist, deployment notes per target, a role check that decides who can create the review package — backed
by a second check in the Click handler — the license key read from secure config (only its status is shown),
a timestamped troubleshooting log, and a package status that turns ready only when every required check is done.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 9/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5089
```

Then open <http://localhost:5089>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

## What to try

| Action | What you should see |
|---|---|
| Start as **Support Agent** | *Review is restricted.* — **Create Review Package** is disabled |
| Pick **Team Lead** or **Admin** | *Deployment review available.* — the button enables |
| Tick the required checks | the status counts up and turns *Ready for review (9 / 9 required checks).* at 9 / 9 |
| **Create Review Package** | a summary: environment, target, version, reviewer, status, optional checks, notes |
| Change **Target** | the deployment notes switch between IIS, Kestrel and Cloud |

## Where things live

| File | What it's for |
|---|---|
| `ReleaseReviewWindow.cs` | `ApplyPermission`, `UpdatePackageStatus`, `btnReviewPackage_Click` (with the server-side re-check), `AddLog` |
| `Services/SecureConfig.cs` | Reads the license key from `WISEJ_LICENSE_KEY` or `Web.config` — never from code |
| `Models/UserAccount.cs` | The signed-in user: name and role |
