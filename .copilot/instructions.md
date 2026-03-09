# zento-api Copilot & AI Agent Instructions

## Purpose
This file provides guidance for GitHub Copilot, Copilot agents, and other AI assistants working in the zento-api repository. It ensures reliable build, publish, and deployment workflows, and helps avoid common CI/CD mistakes.

---

## Build & Publish
- Always use `dotnet publish src/Zento.Api/Zento.Api.csproj -c Release -o ./publish` to generate production output. Do NOT run publish on the solution file.
- Do not deploy the project root or source files—deploy only the published output.

## GitHub Actions Workflow
- The variable `AZURE_WEBAPP_PACKAGE_PATH` is set to `./publish` and must be referenced in:
  - Artifact upload step
  - Deployment step
- The workflow must:
  1. Build the app
  2. Publish the main project file (`src/Zento.Api/Zento.Api.csproj`) to `./publish`
  3. Upload artifact from `./publish`
  4. Deploy artifact from `./publish` to Azure Web App

## Common Pitfalls
- Deploying the wrong directory (project root instead of published output)
- Not referencing workflow variables in all relevant steps
- Missing or misconfigured Azure credentials/secrets

## Best Practices
- Reference workflow variables for paths and app names for maintainability
- Keep build, publish, and deploy steps consistent
- Document any workflow changes in CONTRIBUTING.md

## Quick Checklist for AI Agents
- [ ] Confirm publish output directory is used for artifact and deployment
- [ ] Reference `AZURE_WEBAPP_PACKAGE_PATH` in all relevant steps
- [ ] Validate workflow for errors before deployment
- [ ] Ensure secrets and Azure credentials are set up in GitHub
- [ ] Document any changes to build or deployment scripts

---

## Troubleshooting
- If deployment fails, check workflow logs for path mismatches or missing artifacts
- Validate that the published output was deployed, not the project root
- Review Azure credentials and publish profile setup

---
