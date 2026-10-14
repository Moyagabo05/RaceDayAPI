$base = "http://localhost:5239"

$orgSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
try {
	$regBody = @{ Email = "org@example.com"; Password = "Pass123!"; Role = "Organiser"; FirstName = "Alice"; LastName = "Smith"; OrganisationName = "OrgCo"; PhoneNumber = "01234567890" } | ConvertTo-Json
	$reg = Invoke-RestMethod -Uri "$base/api/auth/register" -Method Post -ContentType "application/json" -Body $regBody -WebSession $orgSession -ErrorAction Stop
	Write-Output "Register org: $reg"

	$loginBody = @{ Email = "org@example.com"; Password = "Pass123!" } | ConvertTo-Json
	$login = Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -ContentType "application/json" -Body $loginBody -WebSession $orgSession -ErrorAction Stop
	Write-Output "Login org: $($login | ConvertTo-Json)"

	$event = @{ Name = "City 10k"; Description = "Fast course"; EventDate = "2026-11-01T09:00:00"; Location = "City Park"; Distance = 10.00; EventType = "Road" }
	$createEvent = Invoke-RestMethod -Uri "$base/api/events" -Method Post -ContentType "application/json" -Body ($event | ConvertTo-Json) -WebSession $orgSession -ErrorAction Stop
	Write-Output "Created event: $($createEvent | ConvertTo-Json)"
	$eventId = $createEvent.EventID

	$cat = @{ CategoryName = "Adult"; MinAge = 18; MaxAge = 39 }
	$addCat = Invoke-RestMethod -Uri "$base/api/events/$eventId/categories" -Method Post -ContentType "application/json" -Body ($cat | ConvertTo-Json) -WebSession $orgSession -ErrorAction Stop
	Write-Output "Added category: $addCat"
} catch {
	Write-Error $_.Exception.Message
	exit 1
}

# Participant actions
$partSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
try {
	$pReg = @{ Email = "part@example.com"; Password = "Pass123!"; Role = "Participant"; FirstName = "Bob"; LastName = "Jones"; DateOfBirth = "1990-01-01"; Gender = "M"; EmergencyContact = "+441234567890"; ClubName = "RunClub" } | ConvertTo-Json
	$regp = Invoke-RestMethod -Uri "$base/api/auth/register" -Method Post -ContentType "application/json" -Body $pReg -WebSession $partSession -ErrorAction Stop
	Write-Output "Register part: $regp"

	$pLogin = @{ Email = "part@example.com"; Password = "Pass123!" } | ConvertTo-Json
	$loginp = Invoke-RestMethod -Uri "$base/api/auth/login" -Method Post -ContentType "application/json" -Body $pLogin -WebSession $partSession -ErrorAction Stop
	Write-Output "Login part: $($loginp | ConvertTo-Json)"

	# Enrol in the category we created (assume first category)
	$enrol = @{ CategoryId = 1 } | ConvertTo-Json
	$en = Invoke-RestMethod -Uri "$base/api/enrolments/events/$eventId/enrol" -Method Post -ContentType "application/json" -Body $enrol -WebSession $partSession -ErrorAction Stop
	Write-Output "Enrol response: $en"

	# Get my enrolments to find enrolment id
	$my = Invoke-RestMethod -Uri "$base/api/enrolments/my-enrolments" -Method Get -WebSession $partSession -ErrorAction Stop
	Write-Output "My enrolments: $($my | ConvertTo-Json)"
	$enrolmentId = $my[0].EnrolmentID

	# Add result as organiser
	$resultBody = @{ FinishTime = "00:45:30"; Position = 5 } | ConvertTo-Json
	$res = Invoke-RestMethod -Uri "$base/api/results/enrolments/$enrolmentId" -Method Post -ContentType "application/json" -Body $resultBody -WebSession $orgSession -ErrorAction Stop
	Write-Output "Add result response: $res"
} catch {
	Write-Error $_.Exception.Message
	exit 1
}
