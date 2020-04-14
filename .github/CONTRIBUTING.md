# Contributing

## What do I need to know to help?

If you are looking to help to with a code contribution our project uses .Net Core 2 framework and is primarily written in C#.
You will also need a [FiveM server](https://docs.fivem.net/docs/server-manual/setting-up-a-server/) to test the resource on.
If you don't feel ready to make a code contribution yet, no problem!
You can also check out the [documentation issues](https://github.com/TGRHavoc/live_map/labels/documentation) that we have.

## How do I make a contribution?

Never made an open source contribution before? Wondering how contributions work in the in our project? Here's a quick rundown!

- Find an issue that you are interested in addressing or a feature that you would like to add.
- Fork the repository associated with the issue to your local GitHub organization. This means that you will have a copy of the repository under `your-GitHub-username/repository-name`.
- Clone the repository to your local machine using `git clone https://github.com/github-username/repository-name.git`.
- Create a new branch for your fix using `git checkout -b branch-name-here`.
  - To make it easier, name the branch something useful. For example `feature-new-pathfinding`.
- Make the appropriate changes for the issue you are trying to address or the feature that you want to add.
  - Try and adhere to [the coding style](coding-style.md) of the project.
- Use `git add insert-paths-of-changed-files-here` to add the file contents of the changed files to the "snapshot" git uses to manage the state of the project, also known as the index.
- Use `git commit` to store the contents of the index with a descriptive message.
  - Make sure to follow [conventional commit standards](https://www.conventionalcommits.org/en/v1.0.0-beta.4/)
- Push the changes to the remote repository using `git push origin branch-name-here`.
- [Submit a pull request to the upstream repository.](https://help.github.com/en/articles/creating-a-pull-request)
- Title the pull request with a short description of the changes made and the issue or bug number associated with your change. For example, you can title an issue like so "Added more log outputting to resolve #4352".
- In the description of the pull request, explain the changes that you made, any issues you think exist with the pull request you made, and any questions you have for the maintainer. It's OK if your pull request is not perfect (no pull request is), the reviewer will be able to help you fix any problems and improve it!
- Wait for the pull request to be reviewed by a maintainer.
- Make changes to the pull request if the reviewing maintainer recommends them.
- Celebrate your success after your pull request is merged!

## Where can I go for help?

If you need help, you can ask questions on the platforms listed:

- Keybase.IO: https://keybase.io/team/havocs_team
