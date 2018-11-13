# last-fm-not-mine-alert-func

[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=chopeen/last-fm-not-mine-alert-func)](https://dependabot.com)

Project with two Azure Functions:

1. [send-alert](send-alert/readme.md) (wakes up periodically; pulls latest tracks from a Last.fm profile; checks them
   against the `not-my-artists` list; emails an alert, if needed)
1. [not-my-artists](not-my-artists/readme.md) (API for management of the 'unexpected artists' list)
