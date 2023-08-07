@Users
Feature: User

Users are used to authenticate, manage data, retrieve information, perform searches, and access resources efficiently and effectively. 

Scenario: As an annomyous User, I want to be able to create an account so that I can manage my data and access resources.
	Given That I provide valid information to register for a user account:
		| UserName   | Email          | Role | Password  | PasswordConfirmation |
		| JohnTester | Test@test.test | User | Test1234! | Test1234!            |
	When submitting a register user form
	Then my account is created

Scenario: As a User, I want to be able to login to my account so that I can manage my data and access resources.
	Given That I provide valid information to login to my user account:
		| UserName  | Email                | Password      | PasswordConfirmation |
		| TestUser1 | User1Email@test.test | UserPassword1 | UserPassword1        |
	When submitting a login user form
	Then I should be logged in

Scenario: As a SuperUser, I want to be able to login to so that I can manage my data and access resources.
	Given That I provide valid information to login to my user account:
		| UserName      | Email                    | Password           | PasswordConfirmation |
		| TestSuperUser | SuperUserEmail@test.test | SuperUserPassword1 | SuperUserPassword1   |
	When submitting a login user form
	Then I should be logged in

Scenario: As an Admin, I want to be able to login to so that I can manage my data and access resources.
	Given That I provide valid information to login to my user account:
		| UserName   | Email                 | Password       | PasswordConfirmation |
		| TestAdmin1 | Admin1Email@test.test | AdminPassword1 | AdminPassword1       |
	When submitting a login user form
	Then I should be logged in

Scenario: As a SuperUser, I want to be able to get all users.
	Given I am logged in as a SuperUser
	When I request all users
	Then I should get all users

Scenario: As a User, I want to be able to log out.
	Given I am logged in as a User
	When I request to log out
	Then I should be logged out

Scenario: As a SuperUser, I want to be able to log out.
	Given I am logged in as a SuperUser
	When I request to log out
	Then I should be logged out

Scenario: As an Admin, I want to be able to log out.
	Given I am logged in as a Admin
	When I request to log out
	Then I should be logged out

Scenario: As a User, I want to be able to update my account information.
	Given I am logged in as a User
	When I request to update my account information
	Then my account information should be updated
	And if I attempt to update another user's account information
	Then I should be denied

Scenario: As a SuperUser or Admin, I want to be able to update any account.
	Given I am logged in as a <UserType>
	When I request to update an account
	Then the account should be updated

	Examples: 
		| UserType	|
		| SuperUser	|
		| Admin		|

Scenario: As a User, I want to be able to delete my account.
	Given I am logged in as a User
	When I request to delete my account
	Then my account should be deleted
	And if I attempt to delete another user's account
	Then I should be denied

Scenario: As a SuperUser or Admin, I want to be able to delete any account.
	Given I am logged in as a <UserType>
	When I request to delete an account
	Then the account should be deleted

	Examples: 
		| UserType	|
		| SuperUser	|
		| Admin		|

Scenario: As any type of User, I would like to be able to be able to refresh my token.
	Given I am logged in as a <UserType>
	When I request to refresh my token
	Then my token should be refreshed

	Examples: 
		| UserType	|
		| SuperUser	|
		| Admin		|
		| User		|

Scenario: As any type of User, if I forget my password, I would like a 'forgot my password' feature to reset my password.
	Given that I provide valid information to reset my password:
		| Email                    | UserName      |
		| SuperUserEmail@test.test |               |
		| TestAdmin1@test.test     |               |
		| User1Email@test.test     |               |
		|                          | TestSuperUser |
		|                          | TestAdmin1    |
		|                          | TestUser1     |
		| SuperUserEmail@test.test | TestSuperUser |
		| TestAdmin1@test.test     | TestAdmin1    |
		| User1Email@test.test     | TestUser1     |
	When I send a forgot password request
	Then I should receive an email with a link to reset my password
	When I click the link
	Then my reset password request should be validated
	And I should be able to reset my password