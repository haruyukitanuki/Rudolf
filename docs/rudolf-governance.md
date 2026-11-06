# Rudolf Governance

**English** | [日本語](./rudolf-governance.ja.md)

These are guidelines pertaining to how Rudolf should handle requests to add new data pointers to its specification, and when and what should trigger a new release.

> [!Note]
> This document is owned by Haruyuki. 
> 
> Please contact me for any clarifications. If you wish to make a PR that modifies this file, please mention and request a review from me.

## The idea of Rudolf
### If Rudolf existed in an ideal world
In an ideal world, Rudolf should aim to support 100% of all producers and adapters, past, present and future. 

Producers adopting Rudolf as their main wire-format for their API should not need to have any loss in data points and resolution they wish to carry across the wire. This means that, as much as possible, a game should not have to place its data inside the extensions section because of a lack of support in the `SimulatorProfile` or `OutputDataFrame` document.

### Rudolf in practice
In practice, Rudolf is unable to cover all edge cases and grounds in order to prevent bloat and unnecessary data points. Every simulator and game out there is unique and programmed in many different ways.

Not everything can be added to Rudolf and some values would need to be placed inside the extensions section which would inadvertently break support in consumers due to unstandardised field naming and producer specific fields.

## Rudolf is prescriptive
Rudolf is designed to be as prescriptive as possible where needed, without being too overbearing, in order to work on a multitude of different consumer software harmoniously without the need for massive code changes.

### Complying with Specifications
In order to be compliant with Rudolf, your game's available set of data points need not be a superset of what Rudolf has. It can be a subset and still be compliant. Rudolf is designed to have fields that are nullable. 

If for any reason, you are unable to populate non-nullable fields, it means that your game or adapter has insufficient data points to use Rudolf and is incapable of becoming Rudolf compliant. You cannot brute force your way into compliance if this is the case.

At any point you find yourself modifying Rudolf in any way, you are breaking the Rudolf specifications, and strictly cannot call your game or adapter Rudolf-compliant. 

This is by design to ensure that consumers are able to still perform useful functions with your supplied data. **Consumers must not break because of your skill issue in data population.**

### "Rudolf-compliant"
You can only claim "Rudolf-compliant" if you strictly follow all prescribed requirements in the Specification document without deviations in behaviour.

**Consumer software maintainers consuming Rudolf who encounter behavioural issues with output from a producer or adapter should not write workarounds to accommodate the unstandardised behaviour. If possible, contact the producer software's maintainer and report it as a bug.** 

It is the producer software's responsibility to maintain specification prescribed behaviour strictly. You should not budge on this, ever.

### How to comply?
Refer to the Specification document for full details.

## Specification proposals
All proposals to make changes to Rudolf's specification must be as specific and perfectionistic as possible.

What this means is that you should not propose vague pointers such as "I want x value".

Suggestions on what to include in proposals:
- State what you want
- What do you need it for
- Why do you need it
- What this value means (explain the terminology, please provide the Japanese equivalent if writing in English)
- What producers (games/simulators) support this value already

### Chasing the last 10%
Rudolf does not chase the last 10% to reach 100% coverage for all possible datapoints as per the "Rudolf in practice" section above.

If a request to add a new data point is proposed, it will be evaluated against the current pool of games/simulators, at least 60% should be able to support the data point and reliably populate a value or otherwise be willing to support it in future.

Case-by-case exceptions apply, but generally the above would be used for evaluation on whether to make a revision to the Specification document.

### Rejected proposals
We do not accept proposals of the following nature:
- Game specific field/section
- Adding a new wire-format document/section
- Repurposing a field for other than its original intended purpose
- When the author clearly does not understand what they are doing or talking about
- Low effort AI slop

## Acceptable LLM-use
We are of the stance that LLM-assisted development helps to increase implementation efficiency and sometimes catch human errors. We do not have any reservations with using LLMs to help write contributions, however, we are against low effort, low quality AI slop.

### Communications
All communications must be done by a human and no text should be written using an LLM. Do not be a meat proxy.

### Quality
#### You are accountable for everything you produce
Rudolf runs on a multitude of software and devices. 

If one of these devices "blows up" because of bad implementation, you cannot blame an LLM for it. You own the mistake.

> **"A computer can never be held accountable, therefore a computer must never make a management decision."**
> 
> &mdash; IBM Training Manual, 1979

#### Do not substitute your own intelligence for Artificial Intelligence
Code and proposal produced by an LLM must have been reviewed by a living, breathing human before being Pull Requested or brought forth.

#### LLM Disclosure
LLM disclosures are prohibited.

This prevents dilution of responsibility and accountability. You should own it fully and take responsibility for it if anything goes wrong.

## Formalisations & Releases

### Library vs. Specifications
Rudolf's main repository (where this document is stored) contains both the specification document and a C# library.

Both are released in tandem. A new release to the C# library does not necessarily mean there is a change to the specifications, it simply means there is code change, mayhaps for example, to address a bug or code clean up.

#### Formalisation

A formalisation is the finalisation of a specification with all changes proposed included. 

It must be done before a release to prevent code vs specification drift.

It is possible for a formalisation to not have a release due to tight release cycles but this should be avoided.

Usually formalisations are done at time of release to prevent confusion. 

### How to handle changes in releases?
#### Non-breaking (Minor)
Changes in this category should be batched rather than released as a single change.

Changes such as:
- new fields
- new sections
- new enum members
- new vocabulary entries

Consumers must tolerate new fields, enum members and vocabulary entries without breaking.

#### Breaking (Major)
Removals, retyping, repurposing and reinterpretation of anything that already exists in the specifications fall under this category. For correcting an errata, see below.

#### Correction to Specifications
Any changes made to the specifications to correct an errata in a current or previous formalisation, which if not corrected, causes a divergence or misinterpretation of the intended use of a function or field, should be corrected immediately with a release.

A correction is never held back by incomplete minor proposals. It takes first priority in the next outgoing release, which may consist of the errata correction alone if nothing else is ready.

### What goes into a release?
In order of priority, if applicable

1. Errata correction for a previous formalisation.
2. Bug fixes addressing issues for a previous formalisation 
3. Minor changes due to specification revisions

The above is assuming the changes are deemed necessary and not made redundant by a specification revision that will be included in the current outgoing cycle.

#### Major releases
A major release contains every breaking change accepted for the cycle. Breaking changes irrelevant to the outgoing cycle wait for the next one. Minor changes do not get added to the release.

If there are corrections to formalisation or bug fixes required in the previous release, they should be done in an earlier release first before doing a major release. This gives software maintainers the chance to update to a minor release rather than needing to commit to a major release in order to get a bug fix for example.

#### Japanese l10n
Japanese documentation should be updated before a release is done. This is strictly to be done since Rudolf's major users are Japanese.

### When to release?
**Be prudent when it comes to deciding to release.**

Rudolf is assumed to be running on software that cannot be frequently updated or their maintainers cannot spend bandwidth on updating themselves to match Rudolf's specifications constantly.

Before each release, all minor proposals and plans must be included in the release and not deferred to a later one. An exception would be a breaking change that is irrelevant to the one in the release cycle that is outgoing. That breaking change should wait for the next cycle.

### Version Schema
Rudolf uses two-part version number for all sections inside the wire-format for every document.

#### Versioning for Rudolf wire-format
```
  major.minor
```

#### Versioning for the Rudolf libraries
```
  specMajor.specMinor.libBuild
```

For example, Rudolf v1.1 would be v1.1.0 (for build 0) and v1.1.24 (for build 24).
