@Contacts
Feature: Contact

Contacts are users that would have left contact information and are interested in hiring.

Scenario: As an anonymous User, I would like to be able to leave my contact information so that I can be contacted by the site owner.
	Given that I provide at least a valid email address:
	| Name     | Email             | Phone      | Message                |
	| John Doe | test123@test.test | 1234567890 | This is a test message |
	When I submit my contact information
	Then I should be able to see a message that my contact information has been received

Scenario: As a User, I would like to be able to get all my contacts
	Given that I am logged in as a User
	When I request to see my contacts
	Then I should recieve a list of my contacts

Scenario: As a SuperUser or Admin, I would like to see all contacts
	Given that I am logged in as a <UserType>
	When I request to see all contacts
	Then I should recieve a list of all contacts

	Examples: 
	| UserType  |
	| SuperUser |
	| Admin     |

Scenario: As a User, I would like to be able to update my contacts
	Given that I am logged in as a User
	When I request to update one of my contacts
	Then my contact should be updated
	And If I try to update a contact that is not mine
	Then I should be denied

Scenario: As a SuperUser or Admin, I would like to be able to update any contact
	Given that I am logged in as a <UserType>
	When I request to update a contact
	Then the contact should be updated

	Examples: 
	| UserType  |
	| SuperUser |
	| Admin     |

Scenario: As a User, I would like to be able to delete my contacts
	Given that I am logged in as a User
	When I request to delete one of my contacts
	Then my contact should be deleted
	And If I try to delete a contact that is not mine
	Then I should be denied

Scenario: As a SuperUser or Admin, I would like to be able to delete any contact
	Given that I am logged in as a <UserType>
	When I request to delete a contact
	Then the contact should be deleted

	Examples: 
	| UserType  |
	| SuperUser |
	| Admin     |

Scenario: As a User, I would like to be able to get a contact by ID
	Given that I am logged in as a User
	When I request to see a contact by ID
	Then I should recieve the contact
	And If I try to get a contact that is not mine
	Then I should be denied

Scenario: As a SuperUser or Admin, I would like to see a contact by ID
	Given that I am logged in as a <UserType>
	When I request to see a contact by ID
	Then I should recieve the contact

	Examples: 
	| UserType  |
	| SuperUser |
	| Admin     |

Scenario: As a User, I would like to be able to get my contacts by email
	Given that I am logged in as a User
	When I request to get contacts by email
	Then I should recieve the contacts
	And If I try to get contacts that are not mine
	Then I should be denied

Scenario: As a SuperUser or Admin, I would like to be able to get contacts by email
	Given that I am logged in as a <UserType>
	When I request to get contacts by email
	Then I should recieve the contacts

	Examples:
	| UserType  |
	| SuperUser |
	| Admin     |

Scenario: As any type of User, I would like to be able to get a list of contacts with a similar name
	Given that I am logged in as a <UserType>
	When I request to get contacts with a similar name to a given name
	Then I should recieve the contacts

	Examples:
	| UserType  |
	| SuperUser |
	| Admin     |
	| User      |