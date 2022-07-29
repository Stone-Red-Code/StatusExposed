Feature: Basic Tests

Scenario: App is running
	Then the app is running

Scenario: First page visit
	Then the navigation contains 5 items

Scenario: User without account access account page
	Then the "account" page is not accessible

Scenario: User without account access admin page
	Then the "admin" page is not accessible