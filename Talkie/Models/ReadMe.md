# Intro

The API has six(6) database models.

* Accounts
* Contacts
* SharedFiles
* Transactions
* Texts
* Messages

## Accounts

Accounts table holds essential information about each user including:

* Number - Account Number
* First Name
* Last Name
* Email Address
* PasswordHash
* PasswordSalt

## Messages

This is a list of all form of communication between two users (including sharing files, text, and transactions)

* SenderId - Account Number of the active user or sender
* RecipientId - Account Number of the recipient or receiver
* TimeSent - Time when the message was sent.
* DateSent - The date when the message was sent
* MessageType:
<br> This describes the type of message sent. It is defined by an enumerator.
* Data - This contains the payload of the message.

## Texts

This is a list of all texts sent between all users. It has a One-to-One relationship with the Messages table.

* MessageId - Foreign Key from Messages
* TextId - Private Key
* Content - The message sent.

## Transaction

This is a list of all transactions made between all users. It has a One-to-One relationship with the Messages table.

* MessageId - Foreign Key from Messages
* TransactionId - Private Key
* Amount
* Description - The message sent.

## Contacts

This table contains lists of contacts for all users (i.e. using a Many-to-Many relationship)

* User
* Friend

## SharedFiles

This contains a list of all files uploaded to the server while communication between contacts

* MessageId - Foreign Key from Messages table
* FileUrl - Url to file on cloud storage
* Description

# <b>Relationships</b>

## One-to-One

* Messages - SharedFiles
* Messages - Transactions
* Messages - Texts

## Many-to-Many

* Accounts - Contacts

## One-to-Many

* Accounts - Messages
