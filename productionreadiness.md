## Production Readiness Checklist

1.1. Engineering practice

* [x] Application repository has to have a README.md
    * [x] Readme should describe purpose of the service
    * [x] Readme should describe usage of datastores
    * [x] Readme should describe any other resiliency patterns/degradation features
    * [x] Anything else that is critical to understanding the service (eg. external partner reliance, caching mechanisms)
    * [x] Readme should contain diagram of service and dependencies
    * [x] Teams Slack Channel Link
    * [x] Teams Email group
* [x] Exposed APIs must be published on API portal (and API-Registry)
* [x] Code is reviewed by another engineer
    * [x] Usually another team member but enterprise architects can also be engaged for this
    * [x] Code reviews done on a MR basis. Smaller the better
* [x] Project must implement unit tests
    * [x] Quality/coverage of unit tests should be part of code review (per feature/MR)
* [x] Project must be using supported languages and libraries
    * [x] Eg. nothing should be EOL at time of shipping
* [x] All secrets in Vault. All non-secret environment variables must be committed to Git.

1.2. Operational Readiness

* [ ] Service level indicators (SLI) and Service level objectives (SLO) need to be identified
    * [ ] SLOs should be set by the business with the end user experience being the focus
    * [ ] Start with something realistic and simple (eg. closer to 3 than 10 SLOs, easily identifiable things)
    * [ ] Production support team can help with this and see this doc for basics
* [x] Application must have been deployed successfully to SIT before moving to higher environments
* [x] Idempotent for restarts (a service can be killed and started multiple times)
* [x] Idempotent for scaling up/down (a service can be autoscaled to multiple instances)
* [x] Application should not have any background job running in the same process as the normal runtime (eg: timed cache clean or DB record purge)
* [ ] Capacity planning must be done before moving to production
* [x] Alerts must be created (by the developing team) for production incidents. Examples:
    * [x] Service is down
    * [x] Amount of 5xx s is high
    * [x] Service is throwing exceptions

1.3. Observability

* [x] Service needs to have a dashboard in Datadog
    * [x] Showing health of the service (error rates, response times, usage)
    * [x] Showing resource usage (CPU/MEM metrics etc)
    * [x] Datastore (Redis, MySQL etc.) usage

1.4. Logging

* [x] Must log in JSON format and have at least the following as fields:
    * [x] Level (eg. Fatal, Error, Warning, Information, Debug, Verbose)
    * [x] Message
    * [x] Timestamp
* [x] LogLevel must tunable via environment variable
* [ ] Logs must exclude PII/PCI data
    * [ ] Passwords, names, phone numbers, emails, PAN numbers, CVV and user logins
* [x] Must have ability to log every request
* [x] All errors/exceptions must be caught and logged
* [x] Logs must follow best practices and formatting standards
* [x] Team must have reviewed logging output in SIT to make sure its at the correct level
    * [x] Logging enough so that errors are caught
    * [x] Logging only things that have value
    * [x] Logging in a structure that is easy for both creating alerts and for humans to understand