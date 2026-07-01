using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.Core;
using ProjectInformation.Core.Models;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class ContactAggregatorTests
{
    [TestMethod]
    public void BuildContacts_MergesProjectsAndCountsMailsPerContact()
    {
        var mails = new[]
        {
            new MailSummary("Anne", "anne@example.com", "P12345 Kickoff", new DateTime(2026, 1, 1), "Celsius", "", "06-12345678", "Engineer"),
            new MailSummary("Anne", "anne@example.com", "Project 23456 Update", new DateTime(2026, 1, 3), "", "0318-123456", "", ""),
            new MailSummary("Bert", "bert@example.com", "No project", new DateTime(2026, 1, 2), "", "", "", "")
        };

        var contacts = ContactAggregator.BuildContacts(mails);

        Assert.AreEqual(2, contacts.Count);
        Assert.AreEqual("Anne", contacts[0].Naam);
        Assert.AreEqual("anne@example.com", contacts[0].Email);
        Assert.AreEqual("P12345; P23456", contacts[0].Projecten);
        Assert.AreEqual(new DateTime(2026, 1, 3), contacts[0].LaatsteContact);
        Assert.AreEqual(2, contacts[0].AantalMails);
        Assert.AreEqual("Celsius", contacts[0].Company);
        Assert.AreEqual("0318-123456", contacts[0].BusinessTelephoneNumber);
        Assert.AreEqual("06-12345678", contacts[0].MobileTelephoneNumber);
        Assert.AreEqual("Engineer", contacts[0].JobTitle);
    }

    [TestMethod]
    public void MergeContacts_MergesContactsByEmailAddress()
    {
        var existing = new[]
        {
            new ContactRecord("Anne", "anne@example.com", "P12345", new DateTime(2026, 1, 1), 2, "Celsius", "0318-123456", "06-12345678", "Engineer")
        };
        var imported = new[]
        {
            new ContactRecord("Anne Latest", "anne@example.com", "P23456", new DateTime(2026, 1, 5), 3, "", "", "", "")
        };

        var contacts = ContactAggregator.MergeContacts(existing, imported);

        Assert.AreEqual(1, contacts.Count);
        Assert.AreEqual("Anne Latest", contacts[0].Naam);
        Assert.AreEqual("P12345; P23456", contacts[0].Projecten);
        Assert.AreEqual(new DateTime(2026, 1, 5), contacts[0].LaatsteContact);
        Assert.AreEqual(5, contacts[0].AantalMails);
        Assert.AreEqual("Celsius", contacts[0].Company);
        Assert.AreEqual("0318-123456", contacts[0].BusinessTelephoneNumber);
        Assert.AreEqual("06-12345678", contacts[0].MobileTelephoneNumber);
        Assert.AreEqual("Engineer", contacts[0].JobTitle);
    }

    [TestMethod]
    public void MergeContacts_KeepsMoreCompleteContactQualityFields()
    {
        var existing = new[]
        {
            new ContactRecord("Anne", "anne@example.com", "P2415; P2415", new DateTime(2026, 1, 1), 1, "Celsius", "", "", "")
        };
        var imported = new[]
        {
            new ContactRecord("Anne", "anne@example.com", "P1234; P2415", new DateTime(2026, 1, 2), 1, "", "0318-123456", "06 12345678", "Projectleider")
        };

        var contacts = ContactAggregator.MergeContacts(existing, imported);

        Assert.AreEqual(1, contacts.Count);
        Assert.AreEqual("Celsius", contacts[0].Company);
        Assert.AreEqual("0318-123456", contacts[0].BusinessTelephoneNumber);
        Assert.AreEqual("06 12345678", contacts[0].MobileTelephoneNumber);
        Assert.AreEqual("Projectleider", contacts[0].JobTitle);
        Assert.AreEqual("P1234; P2415", contacts[0].Projecten);
    }
}
