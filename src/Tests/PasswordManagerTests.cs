using FakeItEasy;
using NUnit.Framework;
using System.Linq;
using System.Security;
using WikiUpload;

namespace Tests
{
    [TestFixture]
    public class PasswordPanagerTests
    {
        private IPasswordStore _store;
        private PasswordDictionary _passwords;
        private PasswordManager _manager;

        [SetUp]
        public void Setup()
        {
            _passwords = new PasswordDictionary();
            _store = A.Fake<IPasswordStore>();
            A.CallTo(() => _store.Load()).Returns(_passwords);
            _manager = new PasswordManager(_store);
        }

        [Test]
        public void When_aPoliticianIsSpeaking_Then_TheyAreLying()
        {
            Assert.Pass();
        }

        [Test]
        public void When_NewPasswordIsAdded_Then_PasswordsAreSaved()
        {
            var password = new SecureString();

            password.AppendChar('a');
            _manager.SavePassword("foo", "bar", password);

            password.AppendChar('b');
            _manager.SavePassword("foo", "bar", password);

            _manager.SavePassword("foo", "baz", password);

            A.CallTo(() => _store.Save(A<PasswordDictionary>._)).MustHaveHappened(3, Times.Exactly);
        }

        [Test]
        public void When_IdenticalPasswordIsAdded_Then_PasswordsAreNotSaved()
        {
            var password = new SecureString();
            password.AppendChar('a');

            _manager.SavePassword("foo", "bar", password);
            _manager.SavePassword("foo", "bar", password);

            A.CallTo(() => _store.Save(A<PasswordDictionary>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_ExistingPasswordIsRemoved_Then_PasswordsAreSaved()
        {
            _manager.SavePassword("foo", "bar", new SecureString());
            Fake.ClearRecordedCalls(_store);
            _manager.RemovePassword("foo", "bar");

            A.CallTo(() => _store.Save(A<PasswordDictionary>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void When_PasswordIsAdded_Then_PasswordIsEncrypted()
        {
            var password = new SecureString();
            password.AppendChar('a');
     
            _manager.SavePassword("foo", "bar", password);

            var savedPassword = _passwords.First().Value;

            Assert.That(savedPassword, Is.Not.EqualTo("a"));
            Assert.That(savedPassword.Length, Is.GreaterThan(1));
        }

        [Test]
        public void When_PasswordIsRetrieved_Then_PasswordIsDecrypted()
        {
            var password = new SecureString();
            password.AppendChar('a');
     
            _manager.SavePassword("foo", "bar", password);
            var retrievedPassword = _manager.GetPassword("foo", "bar");

            Assert.That(retrievedPassword, Is.Not.Null);
            Assert.That(retrievedPassword.Data.Length, Is.EqualTo(1));
            Assert.That(retrievedPassword.Data[0], Is.EqualTo('a'));
        }

        [Test]
        public void When_MissingPasswordIsRetrieved_Then_NullIsReturned()
        {
            var retrievedPassword = _manager.GetPassword("foo", "bar");

            Assert.That(retrievedPassword, Is.Null);
        }

        [Test]
        public void When_PasswordExists_Then_HasPasswordReturnsTrue()
        {
            var password = new SecureString();
            password.AppendChar('a');
            _manager.SavePassword("foo", "bar", password);

            var result = _manager.HasPassword("foo", "bar");

            Assert.That(result, Is.True);
        }

        [Test]
        public void When_DomainPasswordExists_Then_HasPasswordReturnsTrue()
        {
            var password = new SecureString();
            password.AppendChar('a');
            _manager.SaveDomainPassword("a.foobar.com", "bar", password);

            var result1 = _manager.HasPassword("a.foobar.com", "bar");
            var result2 = _manager.HasPassword("b.foobar.com", "bar");

            Assert.That(result1, Is.True);
            Assert.That(result2, Is.True);
        }

        [Test]
        public void When_PasswordDoesNotExist_Then_HasPasswordReturnsFalse()
        {
            var result = _manager.HasPassword("foo", "ba;");

            Assert.That(result, Is.False);
        }

        [Test]
        public void When_DomainPasswordDoesNotExist_Then_HasPasswordReturnsFalse()
        {
            var password = new SecureString();
            password.AppendChar('a');
            _manager.SavePassword("a.foobar.com", "bar", password);

            var result = _manager.HasPassword("b.foobar.com", "bar");

            Assert.That(result, Is.False);
        }

        [Test]
        public void When_ManagerIsCreated_Then_PasswordsAreLoaded()
        {
            var store = A.Fake<IPasswordStore>();
            A.CallTo(() => store.Load()).Returns(_passwords);
            var manager = new PasswordManager(store);

            A.CallTo(() => store.Load()).MustHaveHappened(1, Times.Exactly);
        }


    }
}
