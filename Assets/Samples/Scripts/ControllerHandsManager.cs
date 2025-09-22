using UnityEngine;
using UnityEngine.InputSystem; // ���ο� ����Ƽ �Է� �ý����� ����ϱ� ���� �ʿ��մϴ�.

public class ControllerHandsManager : MonoBehaviour
{
    // �ν����Ϳ��� Input Action Reference�� �Ҵ��� ���� �����Դϴ�.
    // �� �������� Input Action Asset�� ���ǵ� Ư�� �׼�(��: "Ʈ���� ������")�� ����˴ϴ�.
    public InputActionReference triggerActionReference;
    public InputActionReference gripActionReference;

    // �� �𵨿� �ִ� Animator ������Ʈ�� �����Դϴ�.
    // �� Animator�� �Է¿� ���� �� �ִϸ��̼��� ���� ������ Ʈ�� �Ǵ� ���¸� �����մϴ�.
    public Animator handAnimator;

    private void Awake()
    {
        // �� ��ũ��Ʈ�� �پ� �ִ� GameObject�� �ִ� Animator ������Ʈ�� �����ɴϴ�.
        // �Ϲ������� �� �𵨰� Animator�� �����ϴ� GameObject�Դϴ�.
        handAnimator = GetComponent<Animator>();

        // �Է� �׼� ������ �����ϴ� �޼��带 ȣ���մϴ�.
        SetupInputActions();
    }

    /// <summary>
    /// Ʈ���� �� �׸� �Է� �׼ǿ� ���� �����ʸ� �����մϴ�.
    /// �� �׼ǵ��� ����� ��, �� �ִϸ��̼��� ������Ʈ�մϴ�.
    /// </summary>
    void SetupInputActions()
    {
        // �� �Է� �׼� ������ �ν����Ϳ� �Ҵ�Ǿ����� Ȯ���մϴ�.
        if (triggerActionReference != null && gripActionReference != null)
        {
            // Ʈ���� �׼��� 'performed' �̺�Ʈ�� �����մϴ�.
            // Ʈ���Ű� �����ų� ������ �� (�׼� ���� ��), UpdateHandAnimation�� ȣ���մϴ�.
            // 'ctx.ReadValue<float>()'�� �׼��� ���� float ��(��: Ʈ���� ��� �� 0���� 1)�� �����ɴϴ�.
            // performed �̺�Ʈ: �̷��� �Է� �׼��� ������ �Ϸ�ǰų�, Ư�� ������ �������� �� �� �� �߻��մϴ�.
            triggerActionReference.action.performed += ctx => UpdateHandAnimation("Trigger", ctx.ReadValue<float>());

            // �׸� �׼��� 'performed' �̺�Ʈ�� �����մϴ�.
            // Ʈ���ſ� �����ϰ�, "Grip" �ִϸ��̼� �Ķ���͸� ������Ʈ�մϴ�.
            triggerActionReference.action.canceled += ctx => UpdateHandAnimation("Trigger", 0);
            
            // �������� �׼��� ���, �Է��� ������ �� ���� ���������� �缳���ؾ� �Ѵٸ�
            // 'canceled' �̺�Ʈ���� ������ �� �ֽ��ϴ�.
            gripActionReference.action.performed += ctx => UpdateHandAnimation("Grip", ctx.ReadValue<float>());
            gripActionReference.action.canceled += ctx => UpdateHandAnimation("Grip", 0);
        }
        else
        {
            // �Է� �׼� ������ �������� �ʾҴٸ� ����� ����մϴ�.
            // �̷��� �Ǹ� �� �ִϸ��̼��� �۵����� �ʽ��ϴ�.
            Debug.LogWarning("Input Action References are not set in the Inspector");
        }
    }

    /// <summary>
    /// �� Animator�� float �Ķ���͸� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="parameterName">Animator�� float �Ķ���� �̸� (��: "Trigger", "Grip").</param>
    /// <param name="value">�Ķ���Ϳ� ������ float �� (��: 0.0���� 1.0).</param>
    void UpdateHandAnimation(string parameterName, float value)
    {
        // Animator ������Ʈ�� ��ȿ���� Ȯ���� �� �Ķ���͸� �����Ϸ��� �õ��մϴ�.
        if (handAnimator != null)
        {
            // Animator�� float �Ķ���͸� �����մϴ�.
            // �� �Ķ���ʹ� �Ϲ������� Animator Controller�� ������ Ʈ���� �Ϻο��� �մϴ�.
            // �� ������ Ʈ���� �پ��� �� ����(��: ���� ��, �κ������� ���� ��, ������ ���� ��)�� �������մϴ�.
            handAnimator.SetFloat(parameterName, value);
        }
    }

    // --- �Է� �׼� ������ ���� ���� �ֱ� �޼��� ---
    // ������ �����ϰ� ������ ����ȭ�ϱ� ���� �Է� �׼��� �ùٸ��� Ȱ��ȭ �� ��Ȱ��ȭ�ϴ� ���� �߿��մϴ�.
    private void OnEnable()
    {
        // �� GameObject�� Ȱ��ȭ�� �� �׼��� Ȱ��ȭ�մϴ�.
        // �̷��� �ϸ� �Է� ������ �����մϴ�.
        /*
        "?" �����ڴ� Null ���Ǻ� �������Դϴ�.
        if (triggerActionReference != null)
        {
            triggerActionReference.action.Enable();
        }
        �� ������ �ٿ��� ���� �Ʒ��� �����ϴ�.
        */
        triggerActionReference?.action.Enable();
        gripActionReference?.action.Enable();
    }

    private void OnDisable()
    {
        // �� GameObject�� ��Ȱ��ȭ�ǰų� �ı��� �� �׼��� ��Ȱ��ȭ�մϴ�.
        // �̷��� �ϸ� �Է� ������ �����ϰ� �޸� ������ �����մϴ�.
        triggerActionReference?.action.Disable();
        gripActionReference?.action.Disable();
    }
}
